using System.Reflection;
using MertKaan.UAVSimulator.Aircraft;
using MertKaan.UAVSimulator.Landing;
using MertKaan.UAVSimulator.Missions;
using MertKaan.UAVSimulator.Targeting;
using NUnit.Framework;
using UnityEngine;

namespace MertKaan.UAVSimulator.Tests
{
    public sealed class LandingSystemTests
    {
        private GameObject _runwayObject;
        private GameObject _targetObject;

        [SetUp]
        public void SetUp()
        {
            _runwayObject = new GameObject("Runway");
            BoxCollider runway = _runwayObject.AddComponent<BoxCollider>();
            runway.size = new Vector3(100f, 2f, 20f);
            _targetObject = new GameObject("MissionTarget");
            _targetObject.AddComponent<EOTarget>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_runwayObject);
            Object.DestroyImmediate(_targetObject);
        }

        [Test]
        public void Guidance_UsesRunwayThresholdAndCenterline()
        {
            RunwayApproachSnapshot snapshot = RunwayApproachGuidance.CalculateSnapshot(
                new Vector3(-60f, 10f, 5f),
                Vector3.right,
                _runwayObject.GetComponent<BoxCollider>(),
                Vector3.right,
                600f,
                120f);

            Assert.That(snapshot.GuidanceValid, Is.True);
            Assert.That(snapshot.ApproachActive, Is.True);
            Assert.That(snapshot.ThresholdDistanceMeters, Is.EqualTo(10f).Within(0.0001f));
            Assert.That(snapshot.CenterlineErrorMeters, Is.EqualTo(5f).Within(0.0001f));
            Assert.That(snapshot.HeadingErrorDegrees, Is.EqualTo(0f).Within(0.0001f));
            Assert.That(snapshot.HeightAboveRunwayMeters, Is.EqualTo(9f).Within(0.0001f));
        }

        [Test]
        public void Guidance_AfterThresholdIsExplicitlyReported()
        {
            RunwayApproachSnapshot snapshot = RunwayApproachGuidance.CalculateSnapshot(
                new Vector3(-40f, 2f, 0f),
                Vector3.right,
                _runwayObject.GetComponent<BoxCollider>(),
                Vector3.right,
                600f,
                120f);

            Assert.That(snapshot.IsBeforeThreshold, Is.False);
            Assert.That(snapshot.HasPassedThreshold, Is.True);
            Assert.That(snapshot.ThresholdDistanceMeters, Is.LessThan(0f));
            Assert.That(snapshot.ApproachActive, Is.False);
        }

        [Test]
        public void Guidance_HeadingErrorWrapsAcrossZero()
        {
            float error = RunwayApproachGuidance.CalculateHeadingErrorDegrees(
                new Vector3(-0.01745f, 0f, -0.99985f),
                new Vector3(0.01745f, 0f, -0.99985f));

            Assert.That(error, Is.EqualTo(-2f).Within(0.1f));
        }

        [Test]
        public void Guidance_InvalidVerticalLandingDirectionReturnsInvalidSnapshot()
        {
            RunwayApproachSnapshot snapshot = RunwayApproachGuidance.CalculateSnapshot(
                Vector3.zero,
                Vector3.back,
                _runwayObject.GetComponent<BoxCollider>(),
                Vector3.up,
                600f,
                120f);

            Assert.That(snapshot.GuidanceValid, Is.False);
        }

        [Test]
        public void Guidance_CenterlineSignIsPositiveOnRightAndNegativeOnLeft()
        {
            BoxCollider runway = _runwayObject.GetComponent<BoxCollider>();
            RunwayApproachSnapshot right = RunwayApproachGuidance.CalculateSnapshot(
                new Vector3(-60f, 10f, 5f),
                Vector3.right,
                runway,
                Vector3.right,
                600f,
                120f);
            RunwayApproachSnapshot left = RunwayApproachGuidance.CalculateSnapshot(
                new Vector3(-60f, 10f, -5f),
                Vector3.right,
                runway,
                Vector3.right,
                600f,
                120f);

            Assert.That(right.CenterlineErrorMeters, Is.EqualTo(5f).Within(0.0001f));
            Assert.That(left.CenterlineErrorMeters, Is.EqualTo(-5f).Within(0.0001f));
        }

        [Test]
        public void Guidance_InvalidInputsReturnSafeFiniteSentinel()
        {
            RunwayApproachSnapshot nullRunway = RunwayApproachGuidance.CalculateSnapshot(
                Vector3.zero,
                Vector3.right,
                null,
                Vector3.right,
                600f,
                120f);

            Assert.That(nullRunway.GuidanceValid, Is.False);
            Assert.That(float.IsFinite(nullRunway.ThresholdDistanceMeters), Is.True);
            Assert.That(float.IsFinite(nullRunway.CenterlineErrorMeters), Is.True);
            Assert.That(float.IsFinite(nullRunway.HeadingErrorDegrees), Is.True);
        }

        [Test]
        public void Touchdown_RequiresAirborneZeroToPositiveTransition()
        {
            Assert.That(AircraftLandingMonitor.ShouldRecordTouchdown(0, 1, true, false), Is.True);
            Assert.That(AircraftLandingMonitor.ShouldRecordTouchdown(1, 2, true, false), Is.False);
            Assert.That(AircraftLandingMonitor.ShouldRecordTouchdown(0, 1, false, false), Is.False);
            Assert.That(AircraftLandingMonitor.ShouldRecordTouchdown(0, 1, true, true), Is.False);
        }

        [Test]
        public void Touchdown_StoresDownwardComponentOfLastAirborneVelocity()
        {
            Assert.That(
                AircraftLandingMonitor.CalculateTouchdownSinkRate(new Vector3(4f, -4.5f, 12f)),
                Is.EqualTo(4.5f).Within(0.0001f));
            Assert.That(
                AircraftLandingMonitor.CalculateTouchdownSinkRate(new Vector3(0f, 2f, 0f)),
                Is.EqualTo(0f));
        }

        [Test]
        public void TouchdownSinkRate_IgnoresHorizontalVelocity()
        {
            Assert.That(
                AircraftLandingMonitor.CalculateTouchdownSinkRate(new Vector3(80f, -2f, -40f)),
                Is.EqualTo(2f).Within(0.0001f));
        }

        [Test]
        public void SinkWarningAndHardLandingThresholdsAreInclusive()
        {
            Assert.That(AircraftLandingMonitor.IsSinkRateWarning(-3f, 3f), Is.True);
            Assert.That(AircraftLandingMonitor.IsSinkRateWarning(0.1f, 3f), Is.False);
            Assert.That(AircraftLandingMonitor.IsHardLanding(4.5f, 4.5f), Is.True);
            Assert.That(AircraftLandingMonitor.IsHardLanding(4.49f, 4.5f), Is.False);
        }

        [Test]
        public void SinkWarningAlsoCatchesWrongWayDescentNearRunway()
        {
            RunwayApproachSnapshot wrongWay = new RunwayApproachSnapshot(
                true, false, false, true, -507f, 16f, -179f, 28f,
                Vector3.zero, Vector3.right);
            Assert.That(
                AircraftLandingMonitor.ShouldWarnAboutSinkRate(wrongWay, -10.8f, 3f, 50f, 600f),
                Is.True);

            RunwayApproachSnapshot farFromCenterline = new RunwayApproachSnapshot(
                true, false, false, true, -507f, 60f, -179f, 28f,
                Vector3.zero, Vector3.right);
            Assert.That(
                AircraftLandingMonitor.ShouldWarnAboutSinkRate(farFromCenterline, -10.8f, 3f, 50f, 600f),
                Is.False);
            Assert.That(
                AircraftLandingMonitor.ShouldWarnAboutSinkRate(wrongWay, -2f, 3f, 50f, 600f),
                Is.False);
        }

        [Test]
        public void Bounce_IsRecordedOnlyAfterTouchdownContactLoss()
        {
            Assert.That(AircraftLandingMonitor.ShouldRecordBounce(3, 0, true), Is.True);
            Assert.That(AircraftLandingMonitor.ShouldRecordBounce(0, 1, true), Is.False);
            Assert.That(AircraftLandingMonitor.ShouldRecordBounce(3, 0, false), Is.False);
        }

        [Test]
        public void ContactSequence_ReportsMainGearThenNoseWithoutRequiringThatOrder()
        {
            Assert.That(
                AircraftLandingMonitor.DetermineInitialContactSequence(false, true, 1),
                Is.EqualTo(LandingContactSequence.MainGearFirst));
            Assert.That(
                AircraftLandingMonitor.AdvanceContactSequence(
                    LandingContactSequence.MainGearFirst,
                    true,
                    true),
                Is.EqualTo(LandingContactSequence.MainThenNose));
            Assert.That(
                AircraftLandingMonitor.DetermineInitialContactSequence(true, false, 1),
                Is.EqualTo(LandingContactSequence.NoseFirst));
            Assert.That(
                AircraftLandingMonitor.AdvanceContactSequence(
                    LandingContactSequence.NoseFirst,
                    true,
                    false),
                Is.EqualTo(LandingContactSequence.NoseFirst));
        }

        [Test]
        public void NoneWheelIsNotAnOffRunwayContact()
        {
            Assert.That(
                AircraftLandingMonitor.HasOffRunwayContact(
                    GroundSurfaceType.Runway,
                    GroundSurfaceType.Runway,
                    GroundSurfaceType.None),
                Is.False);
        }

        [Test]
        public void TerrainOrOtherWheelContactFailsOffRunway()
        {
            Assert.That(
                AircraftLandingMonitor.HasOffRunwayContact(
                    GroundSurfaceType.Runway,
                    GroundSurfaceType.Terrain,
                    GroundSurfaceType.None),
                Is.True);
            Assert.That(
                AircraftLandingMonitor.HasOffRunwayContact(
                    GroundSurfaceType.Runway,
                    GroundSurfaceType.Other,
                    GroundSurfaceType.Runway),
                Is.True);
        }

        [Test]
        public void RunwayOnlyTouchdownHasNoOffRunwayFailure()
        {
            Assert.That(
                AircraftLandingMonitor.HasOffRunwayContact(
                    GroundSurfaceType.Runway,
                    GroundSurfaceType.Runway,
                    GroundSurfaceType.Runway),
                Is.False);
        }

        [Test]
        public void AirframeTerrainContactFailsOffRunwayButWheelAndRunwayContactsDoNot()
        {
            GameObject bodyObject = new GameObject("BodyCollider");
            GameObject noseWheelObject = new GameObject("NoseWheelCollider");
            GameObject rightWheelObject = new GameObject("RightMainWheelCollider");
            GameObject leftWheelObject = new GameObject("LeftMainWheelCollider");
            GameObject terrainObject = new GameObject("Terrain");

            try
            {
                Collider body = bodyObject.AddComponent<CapsuleCollider>();
                Collider noseWheel = noseWheelObject.AddComponent<SphereCollider>();
                Collider rightWheel = rightWheelObject.AddComponent<SphereCollider>();
                Collider leftWheel = leftWheelObject.AddComponent<SphereCollider>();
                Collider terrain = terrainObject.AddComponent<TerrainCollider>();
                BoxCollider runway = _runwayObject.GetComponent<BoxCollider>();

                Assert.That(
                    AircraftLandingMonitor.ShouldFailOffRunwayAirframeContact(
                        body,
                        terrain,
                        AircraftGroundController.IsWheelCollider(
                            body,
                            noseWheel,
                            rightWheel,
                            leftWheel),
                        runway,
                        new Vector3(60f, 0f, 0f)),
                    Is.True);
                Assert.That(
                    AircraftLandingMonitor.ShouldFailOffRunwayAirframeContact(
                        body,
                        terrain,
                        false,
                        runway,
                        new Vector3(0f, 0f, 9f)),
                    Is.False);
                Assert.That(
                    AircraftLandingMonitor.ShouldFailOffRunwayAirframeContact(
                        noseWheel,
                        terrain,
                        AircraftGroundController.IsWheelCollider(
                            noseWheel,
                            noseWheel,
                            rightWheel,
                            leftWheel),
                        runway,
                        new Vector3(60f, 0f, 0f)),
                    Is.False);
                Assert.That(
                    AircraftLandingMonitor.ShouldFailOffRunwayAirframeContact(
                        body,
                        runway,
                        AircraftGroundController.IsWheelCollider(
                            body,
                            noseWheel,
                            rightWheel,
                            leftWheel),
                        runway,
                        new Vector3(60f, 0f, 0f)),
                    Is.False);
            }
            finally
            {
                Object.DestroyImmediate(bodyObject);
                Object.DestroyImmediate(noseWheelObject);
                Object.DestroyImmediate(rightWheelObject);
                Object.DestroyImmediate(leftWheelObject);
                Object.DestroyImmediate(terrainObject);
            }
        }

        [Test]
        public void RunwayFootprintUsesRotatedScaledColliderAtWingContactPoint()
        {
            BoxCollider runway = _runwayObject.GetComponent<BoxCollider>();
            runway.size = new Vector3(30f, 0.05f, 30f);
            runway.transform.position = new Vector3(-145f, 0.01f, 78f);
            runway.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            runway.transform.localScale = new Vector3(1f, 1f, 15f);

            Assert.That(
                AircraftLandingMonitor.IsInsideRunwayFootprint(
                    runway, new Vector3(-258.44f, 0.09f, 92.33f)),
                Is.True);
            Assert.That(
                AircraftLandingMonitor.IsInsideRunwayFootprint(
                    runway, new Vector3(-258.44f, 0.09f, 93.2f)),
                Is.False);
        }

        [Test]
        public void AirframeLandingCorridorExcludesPerimeterTerrainContacts()
        {
            BoxCollider runway = _runwayObject.GetComponent<BoxCollider>();
            runway.size = new Vector3(30f, 0.05f, 30f);
            runway.transform.position = new Vector3(-145f, 0.01f, 78f);
            runway.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            runway.transform.localScale = new Vector3(1f, 1f, 15f);
            Physics.SyncTransforms();

            Assert.That(
                AircraftLandingMonitor.IsInsideLandingCorridor(
                    runway, Vector3.right, new Vector3(-20.6f, 0.01f, 180f), 600f, 50f),
                Is.False);
            Assert.That(
                AircraftLandingMonitor.IsInsideLandingCorridor(
                    runway, Vector3.right, new Vector3(45f, 0.01f, 120f), 600f, 50f),
                Is.True);
            Assert.That(
                AircraftLandingMonitor.IsInsideLandingCorridor(
                    runway, Vector3.right, new Vector3(-258.44f, 0.09f, 92.33f), 600f, 50f),
                Is.True);
        }

        [Test]
        public void StableLandingRequiresRouteTargetThreeRunwayWheelsAndLowSpeed()
        {
            Assert.That(
                AircraftLandingMonitor.HasStableLandingConditions(
                    3,
                    true,
                    true,
                    true,
                    new Vector3(1.5f, 0.5f, 0f),
                    1.5f,
                    0.5f),
                Is.True);
            Assert.That(
                AircraftLandingMonitor.HasStableLandingConditions(
                    3,
                    true,
                    false,
                    true,
                    Vector3.zero,
                    1.5f,
                    0.5f),
                Is.False);
            Assert.That(
                AircraftLandingMonitor.HasStableLandingConditions(
                    2,
                    true,
                    true,
                    true,
                    Vector3.zero,
                    1.5f,
                    0.5f),
                Is.False);
        }

        [Test]
        public void StableLandingDurationBoundaryIsInclusive()
        {
            Assert.That(AircraftLandingMonitor.HasReachedStableDuration(1.5f, 1.5f), Is.True);
            Assert.That(AircraftLandingMonitor.HasReachedStableDuration(1.49f, 1.5f), Is.False);
        }

        [Test]
        public void StableLandingRequiresTargetObservationAndRouteCompletion()
        {
            Assert.That(
                AircraftLandingMonitor.HasStableLandingConditions(3, true, true, false, Vector3.zero, 1.5f, 0.5f),
                Is.False);
            Assert.That(
                AircraftLandingMonitor.HasStableLandingConditions(3, true, false, true, Vector3.zero, 1.5f, 0.5f),
                Is.False);
        }

        [Test]
        public void StoppedOnRunwayDoesNotRequireMissionCompletion()
        {
            Assert.That(
                AircraftLandingMonitor.HasStoppedOnRunway(3, true, Vector3.zero, 1.5f, 0.5f),
                Is.True);
            Assert.That(
                AircraftLandingMonitor.HasStableLandingConditions(
                    3, true, false, false, Vector3.zero, 1.5f, 0.5f),
                Is.False);
            Assert.That(
                AircraftLandingMonitor.HasStoppedOnRunway(2, true, Vector3.zero, 1.5f, 0.5f),
                Is.False);
            Assert.That(
                AircraftLandingMonitor.HasStoppedOnRunway(3, false, Vector3.zero, 1.5f, 0.5f),
                Is.False);
            Assert.That(
                AircraftLandingMonitor.HasStoppedOnRunway(3, true, new Vector3(2f, 0f, 0f), 1.5f, 0.5f),
                Is.False);
        }

        [Test]
        public void StableLandingRejectsNonFiniteVelocity()
        {
            Assert.That(
                AircraftLandingMonitor.HasStableLandingConditions(
                    3,
                    true,
                    true,
                    true,
                    new Vector3(float.NaN, 0f, 0f),
                    1.5f,
                    0.5f),
                Is.False);
        }

        [Test]
        public void PersistentObservationSurvivesTargetingLossAndCanBeReset()
        {
            GameObject targetingObject = new GameObject("Targeting");
            GameObject observationObject = new GameObject("Observation");
            observationObject.SetActive(false);
            EOTargetingController targeting = targetingObject.AddComponent<EOTargetingController>();
            MissionTargetObservation observation = observationObject.AddComponent<MissionTargetObservation>();
            SetPrivateField(observation, "_targetingController", targeting);
            EOTarget target = _targetObject.GetComponent<EOTarget>();
            MethodInfo handler = typeof(MissionTargetObservation).GetMethod(
                "HandleTargetingSnapshot",
                BindingFlags.Instance | BindingFlags.NonPublic);

            handler.Invoke(observation, new object[] {
                new TargetingSnapshot(TargetingState.Observed, target, target, 2f, 2f, true)
            });
            handler.Invoke(observation, new object[] {
                new TargetingSnapshot(TargetingState.NoTarget, null, null, 0f, 2f, false)
            });
            Assert.That(observation.CurrentSnapshot.IsObserved, Is.True);

            observation.ResetObservation();
            Assert.That(observation.CurrentSnapshot.IsObserved, Is.False);
            Object.DestroyImmediate(targetingObject);
            Object.DestroyImmediate(observationObject);
        }

        private static void SetPrivateField<T>(object target, string fieldName, T value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }
    }
}
