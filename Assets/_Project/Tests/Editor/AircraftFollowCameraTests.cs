using System.Reflection;
using MertKaan.UAVSimulator.CameraSystem;
using NUnit.Framework;
using UnityEngine;

namespace MertKaan.UAVSimulator.Tests
{
    public sealed class AircraftFollowCameraTests
    {
        private const float Tolerance = 0.0001f;

        private GameObject _aircraftObject;
        private GameObject _cameraObject;
        private GameObject _secondaryTargetObject;
        private Camera _camera;
        private AircraftFollowCamera _followCamera;

        [SetUp]
        public void SetUp()
        {
            _aircraftObject = new GameObject("CameraTestAircraft");
            _cameraObject = new GameObject("CameraTestCamera");
            _cameraObject.SetActive(false);
            _camera = _cameraObject.AddComponent<Camera>();
            _followCamera = _cameraObject.AddComponent<AircraftFollowCamera>();

            SetPrivateField("_target", _aircraftObject.transform);
            SetPrivateField("_positionSmoothTime", 0f);
            SetPrivateField("_rotationSharpness", 0f);

            _cameraObject.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_secondaryTargetObject);
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
        public void FullPitchSweep_StaysBehindTargetAtStableDistance()
        {
            Quaternion yawRotation = Quaternion.Euler(0f, 40f, 0f);
            Vector3 yawRight = yawRotation * Vector3.right;
            _aircraftObject.transform.rotation = yawRotation;
            _followCamera.SnapToTarget();
            float initialDistance = Vector3.Distance(
                _followCamera.transform.position,
                _aircraftObject.transform.position);
            float maximumDistanceError = 0f;
            float maximumPositionStep = 0f;
            Vector3 previousCameraPosition = _followCamera.transform.position;

            for (int degree = 1; degree <= 360; degree++)
            {
                _aircraftObject.transform.rotation = Quaternion.AngleAxis(degree, yawRight) * yawRotation;
                InvokePrivateMethod("LateUpdate");

                Vector3 physicalForward = -_aircraftObject.transform.forward;
                Vector3 offset = _followCamera.transform.position - _aircraftObject.transform.position;
                float distance = offset.magnitude;
                maximumDistanceError = Mathf.Max(maximumDistanceError, Mathf.Abs(distance - initialDistance));
                maximumPositionStep = Mathf.Max(
                    maximumPositionStep,
                    Vector3.Distance(_followCamera.transform.position, previousCameraPosition));
                previousCameraPosition = _followCamera.transform.position;

                Assert.That(IsFinite(offset), Is.True);
                Assert.That(Vector3.Dot(offset.normalized, -physicalForward), Is.GreaterThan(0.9f));

                Vector3 viewportPosition = _camera.WorldToViewportPoint(_aircraftObject.transform.position);
                Assert.That(viewportPosition.z, Is.GreaterThan(0f));
                Assert.That(viewportPosition.x, Is.InRange(0.05f, 0.95f));
                Assert.That(viewportPosition.y, Is.InRange(0.05f, 0.95f));
            }

            Assert.That(maximumDistanceError, Is.LessThan(Tolerance));
            Assert.That(maximumPositionStep, Is.LessThan(0.5f));
        }

        [Test]
        public void VerticalFlight_PreservesLastValidHeading()
        {
            Quaternion yawRotation = Quaternion.Euler(0f, 40f, 0f);
            _aircraftObject.transform.rotation = yawRotation;
            _followCamera.SnapToTarget();
            Vector3 previousCameraRight = _followCamera.transform.right;

            Vector3 yawRight = yawRotation * Vector3.right;
            _aircraftObject.transform.rotation = Quaternion.AngleAxis(90f, yawRight) * yawRotation;
            InvokePrivateMethod("LateUpdate");

            Vector3 currentOffset = _followCamera.transform.position - _aircraftObject.transform.position;

            Assert.That(IsFinite(currentOffset), Is.True);
            Assert.That(Vector3.Dot(_followCamera.transform.right, previousCameraRight), Is.GreaterThan(0.999f));
        }

        [Test]
        public void PitchSingularityTransitions_AreContinuous()
        {
            Quaternion yawRotation = Quaternion.Euler(0f, 40f, 0f);
            AssertPitchTransitionIsContinuous(yawRotation, 89f, 90f, 91f);
            AssertPitchTransitionIsContinuous(yawRotation, -89f, -90f, -91f);
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

        [Test]
        public void SetTargetAndSnapToTarget_ResetTrackingHistory()
        {
            SetPrivateField("_positionSmoothTime", 0.15f);
            _followCamera.SnapToTarget();
            _aircraftObject.transform.rotation = Quaternion.Euler(0f, 70f, 0f);
            InvokePrivateMethod("LateUpdate");

            _secondaryTargetObject = new GameObject("CameraTestSecondaryTarget");
            _secondaryTargetObject.transform.position = new Vector3(100f, 10f, -45f);
            _secondaryTargetObject.transform.rotation = Quaternion.Euler(0f, -120f, 0f);
            _followCamera.SetTarget(_secondaryTargetObject.transform, true);
            Vector3 snappedPosition = _followCamera.transform.position;
            InvokePrivateMethod("LateUpdate");

            Assert.That(Vector3.Distance(_followCamera.transform.position, snappedPosition), Is.LessThan(Tolerance));

            _secondaryTargetObject.transform.position += new Vector3(50f, 5f, -20f);
            _followCamera.SnapToTarget();
            snappedPosition = _followCamera.transform.position;
            InvokePrivateMethod("LateUpdate");

            Assert.That(Vector3.Distance(_followCamera.transform.position, snappedPosition), Is.LessThan(Tolerance));
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

        private void AssertPitchTransitionIsContinuous(
            Quaternion yawRotation,
            float firstPitch,
            float secondPitch,
            float thirdPitch)
        {
            Vector3 yawRight = yawRotation * Vector3.right;
            _aircraftObject.transform.position = Vector3.zero;
            _aircraftObject.transform.rotation = yawRotation;
            _followCamera.SnapToTarget();

            _aircraftObject.transform.rotation = Quaternion.AngleAxis(firstPitch, yawRight) * yawRotation;
            InvokePrivateMethod("LateUpdate");
            Vector3 firstPosition = _followCamera.transform.position;
            Quaternion firstRotation = _followCamera.transform.rotation;

            _aircraftObject.transform.rotation = Quaternion.AngleAxis(secondPitch, yawRight) * yawRotation;
            InvokePrivateMethod("LateUpdate");
            Vector3 secondPosition = _followCamera.transform.position;
            Quaternion secondRotation = _followCamera.transform.rotation;

            _aircraftObject.transform.rotation = Quaternion.AngleAxis(thirdPitch, yawRight) * yawRotation;
            InvokePrivateMethod("LateUpdate");
            Vector3 thirdPosition = _followCamera.transform.position;
            Quaternion thirdRotation = _followCamera.transform.rotation;

            Assert.That(Vector3.Distance(firstPosition, secondPosition), Is.LessThan(0.25f));
            Assert.That(Vector3.Distance(secondPosition, thirdPosition), Is.LessThan(0.25f));
            Assert.That(Quaternion.Angle(firstRotation, secondRotation), Is.LessThan(5f));
            Assert.That(Quaternion.Angle(secondRotation, thirdRotation), Is.LessThan(5f));
        }

        private static bool IsFinite(Vector3 value)
        {
            return !float.IsNaN(value.x) && !float.IsInfinity(value.x) &&
                !float.IsNaN(value.y) && !float.IsInfinity(value.y) &&
                !float.IsNaN(value.z) && !float.IsInfinity(value.z);
        }
    }
}
