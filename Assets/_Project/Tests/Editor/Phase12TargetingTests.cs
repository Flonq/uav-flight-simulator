using MertKaan.UAVSimulator.CameraSystem;
using MertKaan.UAVSimulator.Targeting;
using MertKaan.UAVSimulator.UI.Production;
using NUnit.Framework;
using UnityEngine;

namespace MertKaan.UAVSimulator.Tests
{
    public sealed class Phase12TargetingTests
    {
        private GameObject _targetObject;
        private GameObject _otherObject;

        [SetUp]
        public void SetUp()
        {
            _targetObject = new GameObject("MissionTarget");
            _targetObject.AddComponent<BoxCollider>();
            _targetObject.AddComponent<EOTarget>();
            _otherObject = new GameObject("OrdinaryCollider");
            _otherObject.AddComponent<BoxCollider>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_targetObject);
            Object.DestroyImmediate(_otherObject);
        }

        [Test]
        public void CameraModeToggle_AlternatesChaseAndEO()
        {
            Assert.That(CameraModeController.GetNextMode(CameraMode.Chase), Is.EqualTo(CameraMode.EO));
            Assert.That(CameraModeController.GetNextMode(CameraMode.EO), Is.EqualTo(CameraMode.Chase));
        }

        [Test]
        public void GimbalYawAndPitch_AreClampedAtBothLimits()
        {
            Assert.That(EOCameraMath.ClampYaw(120f, 60f), Is.EqualTo(60f));
            Assert.That(EOCameraMath.ClampYaw(-120f, 60f), Is.EqualTo(-60f));
            Assert.That(EOCameraMath.ClampPitch(90f, -45f, 30f), Is.EqualTo(30f));
            Assert.That(EOCameraMath.ClampPitch(-90f, -45f, 30f), Is.EqualTo(-45f));
        }

        [Test]
        public void MouseLook_PositiveHorizontalInputProducesPositiveYawDelta()
        {
            Vector2 delta = EOCameraMath.GetMouseLookDelta(new Vector2(10f, 0f), 0.10f);

            Assert.That(delta.x, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(delta.y, Is.EqualTo(0f).Within(0.0001f));
        }

        [Test]
        public void MouseLook_NegativeHorizontalInputProducesNegativeYawDelta()
        {
            Vector2 delta = EOCameraMath.GetMouseLookDelta(new Vector2(-10f, 0f), 0.10f);

            Assert.That(delta.x, Is.EqualTo(-1f).Within(0.0001f));
            Assert.That(delta.y, Is.EqualTo(0f).Within(0.0001f));
        }

        [Test]
        public void MouseLook_PositiveVerticalInputMovesPhysicalViewUp()
        {
            Vector2 delta = EOCameraMath.GetMouseLookDelta(new Vector2(0f, 10f), 0.10f);

            Assert.That(delta.x, Is.EqualTo(0f).Within(0.0001f));
            Assert.That(delta.y, Is.EqualTo(1f).Within(0.0001f));
        }

        [Test]
        public void MouseLook_NegativeVerticalInputMovesPhysicalViewDown()
        {
            Vector2 delta = EOCameraMath.GetMouseLookDelta(new Vector2(0f, -10f), 0.10f);

            Assert.That(delta.x, Is.EqualTo(0f).Within(0.0001f));
            Assert.That(delta.y, Is.EqualTo(-1f).Within(0.0001f));
        }

        [Test]
        public void MouseLook_IsIndependentOfFrameTime()
        {
            Vector2 delta = EOCameraMath.GetMouseLookDelta(new Vector2(12f, -8f), 0.10f);

            Assert.That(delta.x, Is.EqualTo(1.2f).Within(0.0001f));
            Assert.That(delta.y, Is.EqualTo(-0.8f).Within(0.0001f));
        }

        [Test]
        public void GamepadLook_IsSecondBasedAndUsesTheSameScreenDirections()
        {
            Vector2 fast = EOCameraMath.GetGamepadLookDelta(new Vector2(1f, 1f), 90f, 0.02f);
            Vector2 slow = EOCameraMath.GetGamepadLookDelta(new Vector2(1f, 1f), 90f, 0.01f);

            Assert.That(fast.x, Is.EqualTo(-1.8f).Within(0.0001f));
            Assert.That(fast.y, Is.EqualTo(1.8f).Within(0.0001f));
            Assert.That(fast.x, Is.EqualTo(slow.x * 2f).Within(0.0001f));
            Assert.That(fast.y, Is.EqualTo(slow.y * 2f).Within(0.0001f));
        }

        [Test]
        public void MouseWheel_UsesOneBoundedTwoPointFiveDegreeStepWithoutFrameTime()
        {
            Assert.That(EOCameraMath.GetMouseWheelFovDelta(1f, 2.5f), Is.EqualTo(-2.5f));
            Assert.That(EOCameraMath.GetMouseWheelFovDelta(120f, 2.5f), Is.EqualTo(-2.5f));
            Assert.That(EOCameraMath.GetMouseWheelFovDelta(-1f, 2.5f), Is.EqualTo(2.5f));
        }

        [Test]
        public void GamepadZoom_IsSecondBased()
        {
            float fast = EOCameraMath.GetGamepadZoomDelta(1f, 55f, 0.02f);
            float slow = EOCameraMath.GetGamepadZoomDelta(1f, 55f, 0.01f);

            Assert.That(fast, Is.EqualTo(-1.1f).Within(0.0001f));
            Assert.That(fast, Is.EqualTo(slow * 2f).Within(0.0001f));
        }

        [Test]
        public void ZoomFieldOfView_IsClampedToConfiguredRange()
        {
            Assert.That(EOCameraMath.ClampFieldOfView(5f, 20f, 60f), Is.EqualTo(20f));
            Assert.That(EOCameraMath.ClampFieldOfView(90f, 20f, 60f), Is.EqualTo(60f));
        }

        [Test]
        public void PhysicalForward_UsesAircraftLocalNegativeZ()
        {
            Vector3 forward = EOCameraMath.GetPhysicalForward(Quaternion.identity, 0f, 0f);
            Assert.That(Vector3.Dot(forward, Vector3.back), Is.GreaterThan(0.999f));
        }

        [Test]
        public void PhysicalForward_YawRotatesFromNegativeZAxis()
        {
            Vector3 forward = EOCameraMath.GetPhysicalForward(Quaternion.identity, 90f, 0f);
            Assert.That(Vector3.Dot(forward, Vector3.left), Is.GreaterThan(0.999f));
        }

        [Test]
        public void CenterRayHit_WithEOTargetComponent_IsAccepted()
        {
            EOTarget target = _targetObject.GetComponent<EOTarget>();
            Assert.That(EOTarget.FromCollider(_targetObject.GetComponent<Collider>()), Is.SameAs(target));
        }

        [Test]
        public void OrdinaryCollider_WithoutEOTarget_IsRejected()
        {
            Assert.That(EOTarget.FromCollider(_otherObject.GetComponent<Collider>()), Is.Null);
        }

        [Test]
        public void LockCandidate_RequiresTheSerializedMissionTarget()
        {
            EOTarget target = _targetObject.GetComponent<EOTarget>();
            EOTarget otherTarget = _otherObject.AddComponent<EOTarget>();
            Assert.That(EOTargetingController.CanLockCandidate(target, target), Is.True);
            Assert.That(EOTargetingController.CanLockCandidate(otherTarget, target), Is.False);
        }

        [Test]
        public void ObservationProgress_IsClampedToRequiredDuration()
        {
            Assert.That(EOTargetingController.ClampObservationProgress(-1f, 2f), Is.EqualTo(0f));
            Assert.That(EOTargetingController.ClampObservationProgress(4f, 2f), Is.EqualTo(2f));
        }

        [Test]
        public void TargetingSnapshot_CarriesCandidateLockAndObservationState()
        {
            EOTarget target = _targetObject.GetComponent<EOTarget>();
            var snapshot = new TargetingSnapshot(TargetingState.Observed, target, target, 2f, 2f, true);
            Assert.That(snapshot.HasCandidate, Is.True);
            Assert.That(snapshot.HasLock, Is.True);
            Assert.That(snapshot.IsObserved, Is.True);
            Assert.That(snapshot.IsMissionTarget, Is.True);
        }

        [Test]
        public void CenterRayRange_RejectsNegativeAndOutOfRangeDistances()
        {
            Assert.That(EOCameraRig.IsWithinRange(-0.1f, 100f), Is.False);
            Assert.That(EOCameraRig.IsWithinRange(100.1f, 100f), Is.False);
            Assert.That(EOCameraRig.IsWithinRange(100f, 100f), Is.True);
        }

        [Test]
        public void HudFormatting_SeparatesCameraAndTargetStates()
        {
            Assert.That(ProductionHudFormatting.GetCameraModeLabel(CameraMode.Chase), Is.EqualTo("Chase"));
            Assert.That(ProductionHudFormatting.GetCameraModeLabel(CameraMode.EO), Is.EqualTo("EO"));
            Assert.That(ProductionHudFormatting.GetTargetingStateLabel(TargetingState.NoTarget), Is.EqualTo("No Target"));
            Assert.That(ProductionHudFormatting.GetTargetingStateLabel(TargetingState.Observed), Is.EqualTo("Observed"));
        }
    }
}
