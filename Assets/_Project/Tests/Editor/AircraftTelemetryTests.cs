using System.Reflection;
using MertKaan.UAVSimulator.Aircraft;
using MertKaan.UAVSimulator.InputSystem;
using MertKaan.UAVSimulator.Telemetry;
using NUnit.Framework;
using UnityEngine;

namespace MertKaan.UAVSimulator.Tests
{
    public sealed class AircraftTelemetryTests
    {
        private const float Tolerance = 0.0001f;

        private GameObject _aircraftObject;
        private Rigidbody _rigidbody;
        private AircraftInputReader _inputReader;
        private AircraftEngine _engine;
        private AircraftPhysics _aircraftPhysics;
        private AircraftTelemetry _telemetry;

        [SetUp]
        public void SetUp()
        {
            _aircraftObject = new GameObject("TelemetryTestAircraft");
            _aircraftObject.SetActive(false);
            _rigidbody = _aircraftObject.AddComponent<Rigidbody>();
            _inputReader = _aircraftObject.AddComponent<AircraftInputReader>();
            _engine = _aircraftObject.AddComponent<AircraftEngine>();
            _aircraftPhysics = _aircraftObject.AddComponent<AircraftPhysics>();
            _telemetry = _aircraftObject.AddComponent<AircraftTelemetry>();

            SetPrivateField(_inputReader, "_inputActions", new AircraftInputActions());
            SetPrivateField(_engine, "_inputReader", _inputReader);
            SetPrivateField(_engine, "_rigidbody", _rigidbody);
            SetPrivateField(_aircraftPhysics, "_inputReader", _inputReader);
            SetPrivateField(_aircraftPhysics, "_rigidbody", _rigidbody);
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = false;
            _aircraftObject.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_aircraftObject);
        }

        [Test]
        public void FixedTelemetry_ReportsSpeedAltitudeAndVerticalSpeedWithUnits()
        {
            _rigidbody.position = new Vector3(2f, 123.4f, -8f);
            _rigidbody.linearVelocity = new Vector3(3f, -4f, 12f);

            InvokeFixedUpdate(_aircraftPhysics);
            InvokeFixedUpdate(_telemetry);

            Assert.That(_telemetry.HorizontalGroundSpeedMps, Is.EqualTo(Mathf.Sqrt(153f)).Within(Tolerance));
            Assert.That(_telemetry.AirspeedMps, Is.EqualTo(13f).Within(Tolerance));
            Assert.That(_telemetry.AltitudeMeters, Is.EqualTo(123.4f).Within(Tolerance));
            Assert.That(_telemetry.VerticalSpeedMps, Is.EqualTo(-4f).Within(Tolerance));
        }

        [Test]
        public void Heading_UsesPositiveZZeroAndPositiveX90Convention()
        {
            AssertHeading(0f, 0f);
            AssertHeading(90f, 90f);
            AssertHeading(180f, 180f);
            AssertHeading(270f, 270f);
        }

        [Test]
        public void Heading_North_IsExactlyZeroAndNever360()
        {
            AssertHeading(0f, 0f);

            Assert.That(_telemetry.HeadingDegrees, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(_telemetry.HeadingDegrees, Is.LessThan(360f));
        }

        [Test]
        public void Heading_SmallNorthDeviations_SnapToStableZero()
        {
            AssertHeading(-0.04f, 0f);
            AssertHeading(0.04f, 0f);

            Assert.That(_telemetry.HeadingDegrees, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(_telemetry.HeadingDegrees, Is.Not.EqualTo(360f));
        }

        [Test]
        public void Heading_OutsideNorthSnapWindow_RemainsContinuous()
        {
            AssertHeading(-0.06f, 359.94f);
            Assert.That(_telemetry.HeadingDegrees, Is.EqualTo(359.94f).Within(0.001f));

            AssertHeading(0.06f, 0.06f);
            Assert.That(_telemetry.HeadingDegrees, Is.EqualTo(0.06f).Within(0.001f));
            Assert.That(_telemetry.HeadingDegrees, Is.Not.EqualTo(360f));
        }

        [Test]
        public void Heading_IsInvalidWhenPhysicalNoseIsVertical()
        {
            _rigidbody.rotation = Quaternion.FromToRotation(Vector3.back, Vector3.up);

            InvokeFixedUpdate(_telemetry);

            Assert.That(_telemetry.HeadingValid, Is.False);
            Assert.That(_telemetry.HeadingDegrees, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test]
        public void BodyYaw_IsReportedSeparatelyFromPhysicalHeading()
        {
            _rigidbody.rotation = Quaternion.Euler(0f, 90f, 0f);

            InvokeFixedUpdate(_telemetry);

            Assert.That(_telemetry.YawDegrees, Is.EqualTo(90f).Within(Tolerance));
            Assert.That(_telemetry.HeadingDegrees, Is.EqualTo(270f).Within(Tolerance));
            Assert.That(_telemetry.HeadingValid, Is.True);
        }

        [Test]
        public void BodyAngles_AreReportedInSignedDegrees()
        {
            _rigidbody.rotation = Quaternion.Euler(15f, 30f, -20f);

            InvokeFixedUpdate(_telemetry);

            Assert.That(_telemetry.PitchDegrees, Is.EqualTo(15f).Within(Tolerance));
            Assert.That(_telemetry.RollDegrees, Is.EqualTo(-20f).Within(Tolerance));
            Assert.That(_telemetry.YawDegrees, Is.EqualTo(30f).Within(Tolerance));
        }

        [Test]
        public void BodyYaw_Over180Degrees_IsNormalizedToSignedRange()
        {
            _rigidbody.rotation = Quaternion.Euler(0f, 350f, 0f);

            InvokeFixedUpdate(_telemetry);

            Assert.That(_telemetry.YawDegrees, Is.EqualTo(-10f).Within(Tolerance));
        }

        [Test]
        public void NinetyDegreePitch_AttitudeTelemetryRemainsFinite()
        {
            _rigidbody.rotation = Quaternion.Euler(90f, 0f, 0f);

            InvokeFixedUpdate(_telemetry);

            AssertFiniteAttitudeTelemetry();
            Assert.That(_telemetry.HeadingValid, Is.False);
            Assert.That(_telemetry.HeadingDegrees, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test]
        public void InvertedFlight_AttitudeTelemetryRemainsFinite()
        {
            _rigidbody.rotation = Quaternion.Euler(180f, 45f, 20f);

            InvokeFixedUpdate(_telemetry);

            AssertFiniteAttitudeTelemetry();
        }

        [Test]
        public void EngineTelemetry_ReportsClampedThrottlePercentAndRunningState()
        {
            SetAutoProperty(_engine, "Throttle", 1.25f);
            SetAutoProperty(_engine, "IsRunning", false);
            InvokeFixedUpdate(_telemetry);

            Assert.That(_telemetry.ThrottlePercent, Is.EqualTo(100f).Within(Tolerance));
            Assert.That(_telemetry.EngineRunning, Is.False);

            SetAutoProperty(_engine, "Throttle", 0.375f);
            SetAutoProperty(_engine, "IsRunning", true);
            InvokeFixedUpdate(_telemetry);

            Assert.That(_telemetry.ThrottlePercent, Is.EqualTo(37.5f).Within(Tolerance));
            Assert.That(_telemetry.EngineRunning, Is.True);
        }

        private void AssertHeading(float physicalHeadingDegrees, float expectedHeading)
        {
            Vector3 expectedPhysicalForward = HeadingVector(physicalHeadingDegrees);
            _rigidbody.rotation = Quaternion.LookRotation(-expectedPhysicalForward, Vector3.up);

            Vector3 actualPhysicalForward = _rigidbody.rotation * Vector3.back;
            float actualPhysicalHeading = Mathf.Atan2(
                actualPhysicalForward.x,
                actualPhysicalForward.z) * Mathf.Rad2Deg;
            Assert.That(
                Mathf.Abs(Mathf.DeltaAngle(physicalHeadingDegrees, actualPhysicalHeading)),
                Is.LessThan(0.0001f),
                $"Generated physical nose did not preserve {physicalHeadingDegrees} degrees.");
            Assert.That(actualPhysicalForward.y, Is.EqualTo(0f).Within(0.0001f));

            InvokeFixedUpdate(_telemetry);

            Assert.That(_telemetry.HeadingValid, Is.True);
            Assert.That(_telemetry.HeadingDegrees, Is.EqualTo(expectedHeading).Within(Tolerance));
        }

        private static Vector3 HeadingVector(float headingDegrees)
        {
            float radians = headingDegrees * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radians), 0f, Mathf.Cos(radians));
        }

        private void AssertFiniteAttitudeTelemetry()
        {
            Assert.That(IsFinite(_telemetry.PitchDegrees), Is.True);
            Assert.That(IsFinite(_telemetry.RollDegrees), Is.True);
            Assert.That(IsFinite(_telemetry.YawDegrees), Is.True);
            Assert.That(IsFinite(_telemetry.HeadingDegrees), Is.True);
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        private static void InvokeFixedUpdate(object target)
        {
            MethodInfo method = target.GetType().GetMethod(
                "FixedUpdate",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(target, null);
        }

        private static void SetPrivateField<T>(object target, string fieldName, T value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Missing field: {fieldName}");
            field.SetValue(target, value);
        }

        private static void SetAutoProperty<T>(object target, string propertyName, T value)
        {
            SetPrivateField(target, "<" + propertyName + ">k__BackingField", value);
        }
    }
}
