using System.Reflection;
using MertKaan.UAVSimulator.CameraSystem;
using NUnit.Framework;
using UnityEngine;

namespace MertKaan.UAVSimulator.Tests
{
    public sealed class AircraftFollowCameraTests
    {
        private const float Tolerance = 0.0001f;
        private const float MaxPitchFollowAngle = 20f;

        private GameObject _aircraftObject;
        private GameObject _cameraObject;
        private AircraftFollowCamera _followCamera;

        [SetUp]
        public void SetUp()
        {
            _aircraftObject = new GameObject("CameraTestAircraft");
            _cameraObject = new GameObject("CameraTestCamera");
            _cameraObject.SetActive(false);
            _cameraObject.AddComponent<Camera>();
            _followCamera = _cameraObject.AddComponent<AircraftFollowCamera>();

            SetPrivateField("_target", _aircraftObject.transform);
            SetPrivateField("_maxPitchFollowAngle", MaxPitchFollowAngle);
            SetPrivateField("_positionSmoothTime", 0f);
            SetPrivateField("_rotationSharpness", 0f);

            _cameraObject.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_cameraObject);
            Object.DestroyImmediate(_aircraftObject);
        }

        [Test]
        public void SameHeadingWithDifferentRoll_UsesSameChasePosition()
        {
            Quaternion yawRotation = Quaternion.Euler(0f, 35f, 0f);
            _aircraftObject.transform.rotation = yawRotation;
            _followCamera.SnapToTarget();
            Vector3 unrolledPosition = _followCamera.transform.position;

            Vector3 physicalForward = -(yawRotation * Vector3.forward);
            _aircraftObject.transform.rotation = Quaternion.AngleAxis(60f, physicalForward) * yawRotation;
            _followCamera.SnapToTarget();

            Assert.That(
                Vector3.Distance(_followCamera.transform.position, unrolledPosition),
                Is.LessThan(Tolerance));
        }

        [Test]
        public void LargePitch_UsesConfiguredFollowLimit()
        {
            _aircraftObject.transform.rotation = Quaternion.AngleAxis(60f, Vector3.right);
            _followCamera.SnapToTarget();

            Vector3 expectedPhysicalForward = Quaternion.AngleAxis(MaxPitchFollowAngle, Vector3.right) * Vector3.back;
            Vector3 expectedOffset = Vector3.up * 3f - expectedPhysicalForward * 9f;
            Vector3 actualOffset = _followCamera.transform.position - _aircraftObject.transform.position;

            Assert.That(Vector3.Distance(actualOffset, expectedOffset), Is.LessThan(Tolerance));
            Assert.That(actualOffset.y, Is.GreaterThan(-1f));
        }

        [Test]
        public void VerticalFlight_PreservesLastValidHeading()
        {
            Quaternion yawRotation = Quaternion.Euler(0f, 40f, 0f);
            _aircraftObject.transform.rotation = yawRotation;
            _followCamera.SnapToTarget();
            Vector3 previousHorizontalOffset = Vector3.ProjectOnPlane(
                _followCamera.transform.position - _aircraftObject.transform.position,
                Vector3.up).normalized;

            Vector3 yawRight = yawRotation * Vector3.right;
            _aircraftObject.transform.rotation = Quaternion.AngleAxis(90f, yawRight) * yawRotation;
            InvokePrivateMethod("LateUpdate");

            Vector3 currentOffset = _followCamera.transform.position - _aircraftObject.transform.position;
            Vector3 currentHorizontalOffset = Vector3.ProjectOnPlane(currentOffset, Vector3.up).normalized;

            Assert.That(IsFinite(currentOffset), Is.True);
            Assert.That(currentHorizontalOffset.sqrMagnitude, Is.GreaterThan(0.99f));
            Assert.That(Vector3.Dot(currentHorizontalOffset, previousHorizontalOffset), Is.GreaterThan(0.999f));
        }

        [Test]
        public void ConstantHighSpeedTranslation_UsesFeedForwardForStableOffset()
        {
            SetPrivateField("_positionSmoothTime", 0.15f);
            _followCamera.SnapToTarget();
            Vector3 initialOffset = _followCamera.transform.position - _aircraftObject.transform.position;
            float maximumOffsetError = 0f;
            float distancePerFrame = 70f / 60f;

            for (int frame = 0; frame < 120; frame++)
            {
                _aircraftObject.transform.position += Vector3.right * distancePerFrame;
                InvokePrivateMethod("LateUpdate");

                Vector3 currentOffset = _followCamera.transform.position - _aircraftObject.transform.position;
                maximumOffsetError = Mathf.Max(maximumOffsetError, Vector3.Distance(currentOffset, initialOffset));
            }

            Assert.That(maximumOffsetError, Is.LessThan(0.05f));
        }

        [Test]
        public void PhysicalNegativeZForward_KeepsCameraOnPositiveZSide()
        {
            _aircraftObject.transform.rotation = Quaternion.identity;
            _followCamera.SnapToTarget();

            Vector3 localCameraPosition = _aircraftObject.transform.InverseTransformPoint(_followCamera.transform.position);

            Assert.That(localCameraPosition.z, Is.GreaterThan(0f));
        }

        private void SetPrivateField<T>(string fieldName, T value)
        {
            FieldInfo field = typeof(AircraftFollowCamera).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Missing camera field: {fieldName}");
            field.SetValue(_followCamera, value);
        }

        private void InvokePrivateMethod(string methodName)
        {
            MethodInfo method = typeof(AircraftFollowCamera).GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null, $"Missing camera method: {methodName}");
            method.Invoke(_followCamera, null);
        }

        private static bool IsFinite(Vector3 value)
        {
            return !float.IsNaN(value.x) && !float.IsInfinity(value.x) &&
                !float.IsNaN(value.y) && !float.IsInfinity(value.y) &&
                !float.IsNaN(value.z) && !float.IsInfinity(value.z);
        }
    }
}
