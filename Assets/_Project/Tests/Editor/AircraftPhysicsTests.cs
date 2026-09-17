using System.Reflection;
using MertKaan.UAVSimulator.Aircraft;
using MertKaan.UAVSimulator.InputSystem;
using NUnit.Framework;
using UnityEngine;

namespace MertKaan.UAVSimulator.Tests
{
    public sealed class AircraftPhysicsTests
    {
        private const float AirDensity = 1.225f;
        private const float WingArea = 10f;
        private const float SideForceCoefficient = 0.2f;
        private const float DirectionalStabilityCoefficient = 0.0015f;
        private const float DirectionalStabilityReferenceLength = 4f;
        private const float YawRateDampingCoefficient = 0.02f;
        private const float MaximumSideslipAngle = 45f;
        private const float MaximumYawMoment = 12f;
        private const float MinimumAerodynamicSpeed = 0.5f;
        private const float TestMass = 100f;
        private const float Tolerance = 0.0001f;

        private GameObject _aircraftObject;
        private Rigidbody _rigidbody;
        private AircraftInputReader _inputReader;
        private AircraftEngine _engine;
        private AircraftPhysics _aircraftPhysics;

        [SetUp]
        public void SetUp()
        {
            _aircraftObject = new GameObject("PhysicsTestAircraft");
            _aircraftObject.SetActive(false);
            _rigidbody = _aircraftObject.AddComponent<Rigidbody>();
            _inputReader = _aircraftObject.AddComponent<AircraftInputReader>();
            _engine = _aircraftObject.AddComponent<AircraftEngine>();
            _aircraftPhysics = _aircraftObject.AddComponent<AircraftPhysics>();

            SetPrivateField(_engine, "_inputReader", _inputReader);
            SetPrivateField(_engine, "_rigidbody", _rigidbody);
            SetPrivateField(_aircraftPhysics, "_inputReader", _inputReader);
            SetPrivateField(_aircraftPhysics, "_rigidbody", _rigidbody);
            _rigidbody.useGravity = false;
            _aircraftObject.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_aircraftObject);
        }

        [Test]
        public void ZeroLateralVelocity_ProducesNoSideForce()
        {
            AircraftPhysics.LateralAerodynamicState state = CalculateLateralState(0f, 0f, 60f);

            Assert.That(state.SignedLateralAirspeed, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(state.SideslipAngle, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(state.SideForce, Is.EqualTo(Vector3.zero));
            Assert.That(state.YawMoment, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test]
        public void PositiveAndNegativeSideslip_ProduceSymmetricOpposingForces()
        {
            AircraftPhysics.LateralAerodynamicState positive = CalculateLateralState(20f, 0f, 60f);
            AircraftPhysics.LateralAerodynamicState negative = CalculateLateralState(-20f, 0f, 60f);

            Assert.That(positive.SideslipAngle, Is.EqualTo(-negative.SideslipAngle).Within(Tolerance));
            Assert.That(positive.SideForce.x, Is.EqualTo(-negative.SideForce.x).Within(Tolerance));
            Assert.That(positive.SideForce.x, Is.LessThan(0f));
            Assert.That(negative.SideForce.x, Is.GreaterThan(0f));
            Assert.That(positive.YawMoment, Is.EqualTo(-negative.YawMoment).Within(Tolerance));
        }

        [Test]
        public void SideForce_AlwaysOpposesLateralVelocity()
        {
            float[] lateralSpeeds = { -55f, -20f, 0f, 20f, 55f };

            for (int i = 0; i < lateralSpeeds.Length; i++)
            {
                AircraftPhysics.LateralAerodynamicState state = CalculateLateralState(
                    lateralSpeeds[i],
                    0f,
                    Mathf.Sqrt(55f * 55f + lateralSpeeds[i] * lateralSpeeds[i]));
                Vector3 lateralVelocity = Vector3.right * state.SignedLateralAirspeed;

                Assert.That(Vector3.Dot(state.SideForce, lateralVelocity), Is.LessThanOrEqualTo(Tolerance));
            }
        }

        [Test]
        public void HighSideslip_DecaysAfterControlsAreReleased()
        {
            float betaAfterFourSeconds = SimulateSideslip(0.02f, 200);
            float betaAfterSixSeconds = SimulateSideslip(0.02f, 300);

            Assert.That(betaAfterFourSeconds, Is.LessThan(10f));
            Assert.That(betaAfterSixSeconds, Is.LessThan(5f));
            Assert.That(betaAfterSixSeconds, Is.GreaterThanOrEqualTo(0f));
        }

        [Test]
        public void SideslipDecay_DoesNotOscillateOrReverseAggressively()
        {
            float lateralSpeed = 40f;
            float previousBeta = float.PositiveInfinity;

            for (int step = 0; step < 300; step++)
            {
                float airspeed = Mathf.Sqrt(55f * 55f + lateralSpeed * lateralSpeed);
                AircraftPhysics.LateralAerodynamicState state = CalculateLateralState(
                    lateralSpeed,
                    0f,
                    airspeed);
                float beta = state.SideslipAngle;

                Assert.That(beta, Is.GreaterThanOrEqualTo(-0.001f));
                Assert.That(beta, Is.LessThanOrEqualTo(previousBeta + 0.001f));
                Assert.That(lateralSpeed, Is.GreaterThanOrEqualTo(-0.001f));
                previousBeta = beta;
                lateralSpeed += state.SideForce.x / TestMass * 0.02f;
            }
        }

        [Test]
        public void NeutralStraightFlight_RemainsLaterallyStable()
        {
            float lateralSpeed = 0f;

            for (int step = 0; step < 300; step++)
            {
                float airspeed = Mathf.Sqrt(55f * 55f + lateralSpeed * lateralSpeed);
                AircraftPhysics.LateralAerodynamicState state = CalculateLateralState(
                    lateralSpeed,
                    0f,
                    airspeed);

                lateralSpeed += state.SideForce.x / TestMass * 0.02f;
                Assert.That(lateralSpeed, Is.EqualTo(0f).Within(Tolerance));
                Assert.That(state.YawMoment, Is.EqualTo(0f).Within(Tolerance));
            }
        }

        [Test]
        public void YawCommand_ChangesHeadingAndPostReleaseSideslipRecovers()
        {
            Vector3 yawPositiveTorque = CalculateControlTorqueForYawInput(1f);
            Vector3 yawNegativeTorque = CalculateControlTorqueForYawInput(-1f);
            AircraftPhysics.LateralAerodynamicState postReleaseState = CalculateLateralState(40f, 0f, 68f);

            Assert.That(yawPositiveTorque.y, Is.LessThan(0f));
            Assert.That(yawNegativeTorque.y, Is.GreaterThan(0f));
            Assert.That(postReleaseState.YawMoment, Is.LessThan(0f));
            Assert.That(SimulateSideslip(0.02f, 200), Is.LessThan(10f));
        }

        [Test]
        public void LongitudinalLift_DoesNotScaleFromPureLateralSpeed()
        {
            EvaluateVelocity(Vector3.right * 50f);
            Vector3 pureLateralLift = _aircraftPhysics.LiftForce;
            float lateralAirspeed = _aircraftPhysics.LateralAirspeed;
            float pureLateralSideslip = _aircraftPhysics.SideslipAngle;

            EvaluateVelocity(Vector3.back * 50f);
            Vector3 forwardLift = _aircraftPhysics.LiftForce;

            Assert.That(pureLateralLift.magnitude, Is.LessThan(Tolerance));
            Assert.That(lateralAirspeed, Is.EqualTo(50f).Within(Tolerance));
            Assert.That(pureLateralSideslip, Is.EqualTo(90f).Within(Tolerance));
            Assert.That(forwardLift.magnitude, Is.GreaterThan(0f));
        }

        [Test]
        public void NormalAirspeed_ProducesNormalSpeedState()
        {
            EvaluateVelocity(Vector3.back * 65f);

            Assert.That(_aircraftPhysics.SpeedState, Is.EqualTo(AircraftSpeedState.Normal));
        }

        [Test]
        public void CautionAirspeed_ProducesCautionSpeedState()
        {
            EvaluateVelocity(Vector3.back * 75f);

            Assert.That(_aircraftPhysics.SpeedState, Is.EqualTo(AircraftSpeedState.Caution));
        }

        [Test]
        public void OverspeedEntry_ProducesOverspeedState()
        {
            EvaluateVelocity(Vector3.back * 85f);

            Assert.That(_aircraftPhysics.SpeedState, Is.EqualTo(AircraftSpeedState.Overspeed));
            Assert.That(_aircraftPhysics.ForwardAirspeed, Is.EqualTo(85f).Within(Tolerance));
        }

        [Test]
        public void SpeedStateHysteresis_PreventsThresholdFlicker()
        {
            EvaluateVelocity(Vector3.back * 85f);
            Assert.That(_aircraftPhysics.SpeedState, Is.EqualTo(AircraftSpeedState.Overspeed));

            EvaluateVelocity(Vector3.back * 84f);
            Assert.That(_aircraftPhysics.SpeedState, Is.EqualTo(AircraftSpeedState.Overspeed));

            EvaluateVelocity(Vector3.back * 80f);
            Assert.That(_aircraftPhysics.SpeedState, Is.EqualTo(AircraftSpeedState.Caution));

            EvaluateVelocity(Vector3.back * 75f);
            Assert.That(_aircraftPhysics.SpeedState, Is.EqualTo(AircraftSpeedState.Caution));

            EvaluateVelocity(Vector3.back * 74f);
            Assert.That(_aircraftPhysics.SpeedState, Is.EqualTo(AircraftSpeedState.Normal));
        }

        [Test]
        public void Overspeed_RecoversBelowConfiguredRecoverySpeed()
        {
            EvaluateVelocity(Vector3.back * 90f);
            Assert.That(_aircraftPhysics.SpeedState, Is.EqualTo(AircraftSpeedState.Overspeed));

            EvaluateVelocity(Vector3.back * 79.9f);

            Assert.That(_aircraftPhysics.SpeedState, Is.EqualTo(AircraftSpeedState.Caution));
        }

        [Test]
        public void DragForce_GrowsMonotonicallyAndRemainsFinite()
        {
            float previousDrag = 0f;
            for (int speed = 10; speed <= 100; speed += 10)
            {
                EvaluateVelocity(Vector3.back * speed);
                float drag = _aircraftPhysics.DragForce.magnitude;

                Assert.That(IsFinite(drag), Is.True);
                Assert.That(drag, Is.GreaterThan(previousDrag));
                previousDrag = drag;
            }
        }

        [Test]
        public void FullThrottleStraightFlight_ApproachesNaturalEquilibrium()
        {
            float speed = SimulateFullThrottleSpeed(0.02f, 1000);
            float laterSpeed = SimulateFullThrottleSpeed(0.02f, 2000);

            Assert.That(speed, Is.GreaterThan(60f));
            Assert.That(laterSpeed, Is.LessThan(70f));
            Assert.That(laterSpeed - speed, Is.LessThan(2f));
        }

        [Test]
        public void HighAirspeed_IsReportedWithoutHardVelocityCap()
        {
            EvaluateVelocity(Vector3.back * 100f);

            Assert.That(_aircraftPhysics.ForwardAirspeed, Is.EqualTo(100f).Within(Tolerance));
            Assert.That(_aircraftPhysics.Airspeed, Is.EqualTo(100f).Within(Tolerance));
            Assert.That(_aircraftPhysics.SpeedState, Is.EqualTo(AircraftSpeedState.Overspeed));
        }

        [Test]
        public void PureLateralAirspeed_UsesTotalAirRelativeSpeedForEnvelopeState()
        {
            EvaluateVelocity(Vector3.right * 90f);

            Assert.That(_aircraftPhysics.ForwardAirspeed, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(_aircraftPhysics.Airspeed, Is.EqualTo(90f).Within(Tolerance));
            Assert.That(_aircraftPhysics.SpeedState, Is.EqualTo(AircraftSpeedState.Overspeed));
        }

        [Test]
        public void FullThrottleEquilibrium_RemainsConsistentAcrossSupportedTimesteps()
        {
            float speedAtTenMilliseconds = SimulateFullThrottleSpeed(0.01f, 4000);
            float speedAtTwentyMilliseconds = SimulateFullThrottleSpeed(0.02f, 2000);
            float speedAtFortyMilliseconds = SimulateFullThrottleSpeed(0.04f, 1000);
            float minimumSpeed = Mathf.Min(
                speedAtTenMilliseconds,
                Mathf.Min(speedAtTwentyMilliseconds, speedAtFortyMilliseconds));
            float maximumSpeed = Mathf.Max(
                speedAtTenMilliseconds,
                Mathf.Max(speedAtTwentyMilliseconds, speedAtFortyMilliseconds));

            Assert.That(minimumSpeed, Is.GreaterThan(60f));
            Assert.That(maximumSpeed, Is.LessThan(70f));
            Assert.That(maximumSpeed - minimumSpeed, Is.LessThan(0.1f));
        }

        [Test]
        public void ReverseFlow_DoesNotReceiveFullForwardControlAuthority()
        {
            SetPrivateField(_inputReader, "<Yaw>k__BackingField", 1f);

            EvaluateVelocity(Vector3.back * 50f);
            float forwardControlTorque = _aircraftPhysics.ControlTorque.magnitude;

            EvaluateVelocity(Vector3.forward * 50f);
            float reverseControlTorque = _aircraftPhysics.ControlTorque.magnitude;

            Assert.That(forwardControlTorque, Is.GreaterThan(0f));
            Assert.That(reverseControlTorque, Is.GreaterThan(0f));
            Assert.That(reverseControlTorque, Is.LessThan(forwardControlTorque * 0.5f));
        }

        [Test]
        public void ExtremeAttitudes_RemainFinite()
        {
            Quaternion[] rotations =
            {
                Quaternion.identity,
                Quaternion.Euler(90f, 0f, 0f),
                Quaternion.Euler(-90f, 0f, 0f),
                Quaternion.Euler(180f, 45f, 10f)
            };
            Vector3[] velocities =
            {
                Vector3.right * 60f,
                Vector3.back * 60f,
                Vector3.forward * 60f,
                new Vector3(80f, 40f, 0f),
                Vector3.zero
            };

            for (int rotationIndex = 0; rotationIndex < rotations.Length; rotationIndex++)
            {
                for (int velocityIndex = 0; velocityIndex < velocities.Length; velocityIndex++)
                {
                    EvaluateVelocity(velocities[velocityIndex], rotations[rotationIndex]);

                    Assert.That(IsFinite(_aircraftPhysics.AirRelativeVelocity), Is.True);
                    Assert.That(IsFinite(_aircraftPhysics.LiftForce), Is.True);
                    Assert.That(IsFinite(_aircraftPhysics.DragForce), Is.True);
                    Assert.That(IsFinite(_aircraftPhysics.ControlTorque), Is.True);
                    Assert.That(IsFinite(_aircraftPhysics.LateralAirspeed), Is.True);
                    Assert.That(IsFinite(_aircraftPhysics.SideslipAngle), Is.True);
                }
            }
        }

        [Test]
        public void FixedTimestepConsistency_RemainsCloseAcrossSupportedSteps()
        {
            float betaAtTenMilliseconds = SimulateSideslip(0.01f, 400);
            float betaAtTwentyMilliseconds = SimulateSideslip(0.02f, 200);
            float betaAtFortyMilliseconds = SimulateSideslip(0.04f, 100);
            float minimumBeta = Mathf.Min(betaAtTenMilliseconds, Mathf.Min(betaAtTwentyMilliseconds, betaAtFortyMilliseconds));
            float maximumBeta = Mathf.Max(betaAtTenMilliseconds, Mathf.Max(betaAtTwentyMilliseconds, betaAtFortyMilliseconds));

            Assert.That(maximumBeta - minimumBeta, Is.LessThan(0.2f));
        }

        private AircraftPhysics.LateralAerodynamicState CalculateLateralState(
            float lateralAirspeed,
            float localYawRate,
            float airspeed)
        {
            return AircraftPhysics.CalculateLateralAerodynamics(
                Vector3.right,
                AirDensity,
                WingArea,
                airspeed,
                lateralAirspeed,
                localYawRate,
                SideForceCoefficient,
                DirectionalStabilityCoefficient,
                DirectionalStabilityReferenceLength,
                YawRateDampingCoefficient,
                MaximumSideslipAngle,
                MaximumYawMoment,
                MinimumAerodynamicSpeed);
        }

        private float SimulateSideslip(float timestep, int steps)
        {
            float lateralSpeed = 40f;
            const float forwardSpeed = 55f;

            for (int step = 0; step < steps; step++)
            {
                float airspeed = Mathf.Sqrt(forwardSpeed * forwardSpeed + lateralSpeed * lateralSpeed);
                AircraftPhysics.LateralAerodynamicState state = CalculateLateralState(
                    lateralSpeed,
                    0f,
                    airspeed);
                lateralSpeed += state.SideForce.x / TestMass * timestep;
            }

            float finalAirspeed = Mathf.Sqrt(forwardSpeed * forwardSpeed + lateralSpeed * lateralSpeed);
            return AircraftPhysics.CalculateSideslipAngle(lateralSpeed, finalAirspeed);
        }

        private float SimulateFullThrottleSpeed(float timestep, int steps)
        {
            float originalFixedDeltaTime = Time.fixedDeltaTime;
            try
            {
                Time.fixedDeltaTime = timestep;
                SetAutoProperty(_inputReader, "ThrottleInput", 1f);
                SetAutoProperty(_inputReader, "Pitch", 0f);
                SetAutoProperty(_inputReader, "Roll", 0f);
                SetAutoProperty(_inputReader, "Yaw", 0f);
                SetAutoProperty(_engine, "Throttle", 0f);
                SetAutoProperty(_engine, "Rpm", 0f);
                SetAutoProperty(_engine, "ThrustNewtons", 0f);
                SetAutoProperty(_engine, "IsRunning", true);

                float speed = 0f;
                for (int step = 0; step < steps; step++)
                {
                    InvokeFixedUpdate(_engine);
                    _rigidbody.linearVelocity = Vector3.back * speed;
                    InvokeFixedUpdate(_aircraftPhysics);
                    float forwardForce = _engine.ThrustNewtons +
                        Vector3.Dot(_aircraftPhysics.DragForce, Vector3.back);
                    speed = Mathf.Max(
                        0f,
                        speed + forwardForce / TestMass * timestep);
                }

                return speed;
            }
            finally
            {
                Time.fixedDeltaTime = originalFixedDeltaTime;
            }
        }

        private Vector3 CalculateControlTorqueForYawInput(float yawInput)
        {
            SetPrivateField(_inputReader, "<Yaw>k__BackingField", yawInput);
            MethodInfo method = typeof(AircraftPhysics).GetMethod(
                "CalculateControlTorque",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            return (Vector3)method.Invoke(_aircraftPhysics, new object[] { 1f });
        }

        private void EvaluateVelocity(Vector3 velocity, Quaternion rotation = default)
        {
            if (rotation == default)
            {
                rotation = Quaternion.identity;
            }

            _aircraftObject.transform.rotation = rotation;
            _rigidbody.linearVelocity = velocity;
            MethodInfo method = typeof(AircraftPhysics).GetMethod(
                "FixedUpdate",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(_aircraftPhysics, null);
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
            FieldInfo field = target.GetType().GetField(
                "<" + propertyName + ">k__BackingField",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Missing property backing field: {propertyName}");
            field.SetValue(target, value);
        }

        private static void InvokeFixedUpdate(object target)
        {
            MethodInfo method = target.GetType().GetMethod(
                "FixedUpdate",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(target, null);
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        private static bool IsFinite(Vector3 value)
        {
            return IsFinite(value.x) && IsFinite(value.y) && IsFinite(value.z);
        }
    }
}
