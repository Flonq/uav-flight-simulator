using System;
using MertKaan.UAVSimulator.Aircraft;
using MertKaan.UAVSimulator.Missions;
using UnityEngine;

namespace MertKaan.UAVSimulator.Landing
{
    /// <summary>
    /// Evaluates touchdown and landing outcomes from the existing ground
    /// controller telemetry. It does not apply forces or change wheel settings.
    /// Prototype thresholds are intentionally serialized for later calibration.
    /// </summary>
    [DefaultExecutionOrder(300)]
    [DisallowMultipleComponent]
    public sealed class AircraftLandingMonitor : MonoBehaviour
    {
        private const float SinkWarningCenterlineWindowMeters = 50f;
        private const float OffRunwayAirframeCenterlineWindowMeters = 50f;

        [Header("References")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private AircraftGroundController _groundController;
        [SerializeField] private MissionManager _missionManager;
        [SerializeField] private MissionTargetObservation _targetObservation;
        [SerializeField] private RunwayApproachGuidance _approachGuidance;

        [Header("Prototype Thresholds")]
        [SerializeField, Min(0f)] private float _sinkRateWarningMps = 3f;
        [SerializeField, Min(0f)] private float _hardLandingSinkRateMps = 4.5f;
        [SerializeField, Min(0f)] private float _stableGroundSpeedMps = 1.5f;
        [SerializeField, Min(0f)] private float _stableVerticalSpeedMps = 0.5f;
        [SerializeField, Min(0f)] private float _stableDurationSeconds = 1.5f;
        [SerializeField, Min(0f)] private float _sinkWarningHeightWindowMeters = 50f;

        private LandingSnapshot _currentSnapshot = LandingSnapshot.Initial;
        private LandingState _state = LandingState.Inactive;
        private int _previousGroundedWheelCount;
        private bool _hasBeenAirborne;
        private bool _hasTouchdown;
        private float _touchdownTimeSeconds;
        private int _touchdownWheelCount;
        private float _touchdownSinkRateMps;
        private Vector3 _lastAirborneVelocity;
        private LandingContactSequence _contactSequence;
        private GroundSurfaceType _firstNoseWheelSurface;
        private GroundSurfaceType _firstRightMainWheelSurface;
        private GroundSurfaceType _firstLeftMainWheelSurface;
        private bool _hasBounced;
        private int _bounceCount;
        private float _stableDurationSecondsAccumulated;
        private bool _hardLanding;
        private bool _offRunway;
        private bool _validationErrorReported;

        public LandingSnapshot CurrentSnapshot => _currentSnapshot;
        public LandingState State => _state;
        public LandingGearState GearState => LandingGearState.FixedDown;
        public float SinkRateWarningMps => _sinkRateWarningMps;
        public float HardLandingSinkRateMps => _hardLandingSinkRateMps;
        public float StableDurationSeconds => _stableDurationSeconds;
        public string OffRunwayFailureEvidence { get; private set; } = string.Empty;
        public event Action<LandingSnapshot> SnapshotUpdated;

        private void Reset()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _groundController = GetComponent<AircraftGroundController>();
        }

        private void OnValidate()
        {
            _sinkRateWarningMps = Mathf.Max(0f, _sinkRateWarningMps);
            _hardLandingSinkRateMps = Mathf.Max(_sinkRateWarningMps, _hardLandingSinkRateMps);
            _stableGroundSpeedMps = Mathf.Max(0f, _stableGroundSpeedMps);
            _stableVerticalSpeedMps = Mathf.Max(0f, _stableVerticalSpeedMps);
            _stableDurationSeconds = Mathf.Max(0f, _stableDurationSeconds);
            _sinkWarningHeightWindowMeters = Mathf.Max(0f, _sinkWarningHeightWindowMeters);
        }

        private void Awake()
        {
            if (!HasValidReferences())
            {
                ReportValidationError();
                enabled = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision == null ||
                _groundController == null ||
                _missionManager == null ||
                !_missionManager.IsMissionStarted ||
                IsTerminalState(_state))
            {
                return;
            }

            for (int i = 0; i < collision.contactCount; i++)
            {
                ContactPoint contact = collision.GetContact(i);
                if (!ShouldFailOffRunwayAirframeContact(
                        contact.thisCollider,
                        contact.otherCollider,
                        _groundController.IsWheelCollider(contact.thisCollider),
                        _approachGuidance.RunwayCollider,
                        contact.point) ||
                    !IsInsideLandingCorridor(
                        _approachGuidance.RunwayCollider,
                        _approachGuidance.LandingDirectionWorld,
                        contact.point,
                        _approachGuidance.ApproachDistanceMeters,
                        OffRunwayAirframeCenterlineWindowMeters))
                {
                    continue;
                }

                RecordOffRunwayFailure(
                    $"airframe:{contact.thisCollider.name}/surface:{contact.otherCollider.name}",
                    contact.point);
                PublishSnapshot(true);
                return;
            }
        }

        private void FixedUpdate()
        {
            if (!HasValidReferences())
            {
                ReportValidationError();
                return;
            }

            AircraftGroundController ground = _groundController;
            int groundedWheelCount = ground.GroundedWheelCount;
            bool missionStarted = _missionManager.IsMissionStarted;
            if (!missionStarted)
            {
                _state = LandingState.Inactive;
                _previousGroundedWheelCount = groundedWheelCount;
                PublishSnapshot(false);
                return;
            }

            bool hasLostContact = _hasTouchdown &&
                _previousGroundedWheelCount > 0 &&
                groundedWheelCount == 0;
            bool touchdown = ShouldRecordTouchdown(
                _previousGroundedWheelCount,
                groundedWheelCount,
                _hasBeenAirborne,
                _hasTouchdown);

            if (groundedWheelCount == 0)
            {
                _hasBeenAirborne = true;
                _lastAirborneVelocity = _rigidbody.linearVelocity;
            }

            bool touchdownRecorded = false;
            if (!IsTerminalState(_state))
            {
                if (touchdown)
                {
                    RecordTouchdown();
                    touchdownRecorded = true;
                }
                else if (hasLostContact)
                {
                    _hasBounced = true;
                    _bounceCount++;
                    _stableDurationSecondsAccumulated = 0f;
                }

                if (_hasTouchdown && !touchdownRecorded)
                {
                    UpdateContactSequence();
                    EvaluateTouchdownAndOutcome(groundedWheelCount);
                }
                else if (!_hasTouchdown)
                {
                    if (!_hasBeenAirborne && groundedWheelCount > 0)
                    {
                        _state = LandingState.GroundReady;
                    }
                    else
                    {
                        _state = _approachGuidance.CurrentSnapshot.ApproachActive
                            ? LandingState.Approach
                            : LandingState.Airborne;
                    }
                }
            }

            _previousGroundedWheelCount = groundedWheelCount;
            PublishSnapshot(missionStarted);
        }

        private void RecordTouchdown()
        {
            _hasTouchdown = true;
            _touchdownTimeSeconds = Time.time;
            _touchdownWheelCount = _groundController.GroundedWheelCount;
            _touchdownSinkRateMps = CalculateTouchdownSinkRate(_lastAirborneVelocity);
            _contactSequence = DetermineInitialContactSequence(
                _groundController.NoseWheelGrounded,
                _groundController.RightMainWheelGrounded || _groundController.LeftMainWheelGrounded,
                _touchdownWheelCount);
            _firstNoseWheelSurface = _groundController.NoseWheelSurface;
            _firstRightMainWheelSurface = _groundController.RightMainWheelSurface;
            _firstLeftMainWheelSurface = _groundController.LeftMainWheelSurface;
            _state = LandingState.InitialContact;

            if (_touchdownSinkRateMps >= _hardLandingSinkRateMps)
            {
                _hardLanding = true;
                _state = LandingState.FailedHardLanding;
            }
            else if (HasOffRunwayContact(
                _firstNoseWheelSurface,
                _firstRightMainWheelSurface,
                _firstLeftMainWheelSurface))
            {
                RecordOffRunwayFailure("touchdown-wheel-surface", _rigidbody.position);
            }
        }

        private void UpdateContactSequence()
        {
            _contactSequence = AdvanceContactSequence(
                _contactSequence,
                _groundController.NoseWheelGrounded,
                _groundController.RightMainWheelGrounded || _groundController.LeftMainWheelGrounded);
        }

        private void EvaluateTouchdownAndOutcome(int groundedWheelCount)
        {
            if (IsTerminalState(_state))
            {
                return;
            }

            if (HasOffRunwayContact(
                _groundController.NoseWheelSurface,
                _groundController.RightMainWheelSurface,
                _groundController.LeftMainWheelSurface))
            {
                RecordOffRunwayFailure("rollout-wheel-surface", _rigidbody.position);
                return;
            }

            if (_state == LandingState.InitialContact)
            {
                _state = LandingState.GroundRoll;
            }

            if (groundedWheelCount == 0)
            {
                _stableDurationSecondsAccumulated = 0f;
                _state = LandingState.GroundRoll;
                return;
            }

            bool stoppedOnRunway = HasStoppedOnRunway(
                groundedWheelCount,
                _groundController.AllContactingWheelsOnRunway,
                _rigidbody.linearVelocity,
                _stableGroundSpeedMps,
                _stableVerticalSpeedMps);
            if (!stoppedOnRunway)
            {
                _stableDurationSecondsAccumulated = 0f;
                _state = LandingState.GroundRoll;
                return;
            }

            _state = LandingState.Stopped;
            if (!_missionManager.RouteCompleted || !_targetObservation.CurrentSnapshot.IsObserved)
            {
                _stableDurationSecondsAccumulated = 0f;
                return;
            }

            _stableDurationSecondsAccumulated += Time.fixedDeltaTime;
            if (HasReachedStableDuration(_stableDurationSecondsAccumulated, _stableDurationSeconds))
            {
                _state = LandingState.Successful;
            }
        }

        private void PublishSnapshot(bool missionStarted)
        {
            RunwayApproachSnapshot approach = _approachGuidance.CurrentSnapshot;
            bool sinkWarning = missionStarted && ShouldWarnAboutSinkRate(
                approach,
                _rigidbody.linearVelocity.y,
                _sinkRateWarningMps,
                _sinkWarningHeightWindowMeters,
                _approachGuidance.ApproachDistanceMeters);

            bool targetObserved = _targetObservation.CurrentSnapshot.IsObserved;
            _currentSnapshot = new LandingSnapshot(
                _state,
                LandingGearState.FixedDown,
                _groundController.HasValidWheelReferences,
                _contactSequence,
                _hasTouchdown,
                _touchdownTimeSeconds,
                _touchdownWheelCount,
                _touchdownSinkRateMps,
                _lastAirborneVelocity,
                _firstNoseWheelSurface,
                _firstRightMainWheelSurface,
                _firstLeftMainWheelSurface,
                _groundController.GroundedWheelCount,
                _groundController.NoseWheelSurface,
                _groundController.RightMainWheelSurface,
                _groundController.LeftMainWheelSurface,
                _hasBounced,
                _bounceCount,
                _stableDurationSecondsAccumulated,
                sinkWarning,
                _missionManager.RouteCompleted,
                targetObserved,
                _hardLanding,
                _offRunway,
                _state == LandingState.Successful);
            SnapshotUpdated?.Invoke(_currentSnapshot);
        }

        private void RecordOffRunwayFailure(string source, Vector3 contactPoint)
        {
            _offRunway = true;
            _state = LandingState.FailedOffRunway;
            MissionSnapshot mission = _missionManager.CurrentSnapshot;
            RunwayApproachSnapshot approach = _approachGuidance.CurrentSnapshot;
            OffRunwayFailureEvidence =
                $"source={source} fixedTime={Time.fixedTime:F2} " +
                $"mission={mission.State}/{mission.CompletedWaypointCount}/{mission.TotalWaypointCount} " +
                $"airborne={_hasBeenAirborne} touchdown={_hasTouchdown} " +
                $"aircraft={_rigidbody.position} contact={contactPoint} " +
                $"RWY={approach.ThresholdDistanceMeters:F1} CL={approach.CenterlineErrorMeters:F1} " +
                $"wheels={_groundController.NoseWheelSurface}/{_groundController.RightMainWheelSurface}/{_groundController.LeftMainWheelSurface}";
            Debug.Log($"Landing off-runway contact: {OffRunwayFailureEvidence}", this);
        }

        public static bool ShouldRecordTouchdown(
            int previousGroundedWheelCount,
            int currentGroundedWheelCount,
            bool hasBeenAirborne,
            bool hasTouchdown)
        {
            return !hasTouchdown &&
                hasBeenAirborne &&
                previousGroundedWheelCount == 0 &&
                currentGroundedWheelCount > 0;
        }

        public static bool ShouldRecordBounce(
            int previousGroundedWheelCount,
            int currentGroundedWheelCount,
            bool hasTouchdown)
        {
            return hasTouchdown && previousGroundedWheelCount > 0 && currentGroundedWheelCount == 0;
        }

        public static LandingContactSequence DetermineInitialContactSequence(
            bool noseWheelGrounded,
            bool anyMainWheelGrounded,
            int groundedWheelCount)
        {
            if (groundedWheelCount <= 0)
            {
                return LandingContactSequence.None;
            }

            if (noseWheelGrounded && anyMainWheelGrounded)
            {
                return LandingContactSequence.Simultaneous;
            }

            return noseWheelGrounded
                ? LandingContactSequence.NoseFirst
                : LandingContactSequence.MainGearFirst;
        }

        public static LandingContactSequence AdvanceContactSequence(
            LandingContactSequence current,
            bool noseWheelGrounded,
            bool anyMainWheelGrounded)
        {
            if (current == LandingContactSequence.MainGearFirst && noseWheelGrounded)
            {
                return LandingContactSequence.MainThenNose;
            }

            if (current == LandingContactSequence.NoseFirst && anyMainWheelGrounded)
            {
                return LandingContactSequence.NoseThenMain;
            }

            return current;
        }

        public static float CalculateTouchdownSinkRate(Vector3 lastAirborneVelocity)
        {
            float downwardSpeed = -Vector3.Dot(lastAirborneVelocity, Vector3.up);
            return float.IsFinite(downwardSpeed) ? Mathf.Max(0f, downwardSpeed) : 0f;
        }

        public static bool IsSinkRateWarning(float verticalSpeedMps, float warningThresholdMps)
        {
            return verticalSpeedMps <= -Mathf.Max(0f, warningThresholdMps);
        }

        public static bool ShouldWarnAboutSinkRate(
            RunwayApproachSnapshot approach,
            float verticalSpeedMps,
            float warningThresholdMps,
            float heightWindowMeters,
            float distanceWindowMeters)
        {
            return approach.GuidanceValid &&
                approach.HeightAboveRunwayMeters >= 0f &&
                approach.HeightAboveRunwayMeters <= Mathf.Max(0f, heightWindowMeters) &&
                Mathf.Abs(approach.CenterlineErrorMeters) <= SinkWarningCenterlineWindowMeters &&
                Mathf.Abs(approach.ThresholdDistanceMeters) <= Mathf.Max(0f, distanceWindowMeters) &&
                IsSinkRateWarning(verticalSpeedMps, warningThresholdMps);
        }

        public static bool IsHardLanding(float touchdownSinkRateMps, float hardLandingThresholdMps)
        {
            return touchdownSinkRateMps >= Mathf.Max(0f, hardLandingThresholdMps);
        }

        public static bool HasOffRunwayContact(
            GroundSurfaceType noseWheelSurface,
            GroundSurfaceType rightMainWheelSurface,
            GroundSurfaceType leftMainWheelSurface)
        {
            return IsOffRunwaySurface(noseWheelSurface) ||
                IsOffRunwaySurface(rightMainWheelSurface) ||
                IsOffRunwaySurface(leftMainWheelSurface);
        }

        public static bool ShouldFailOffRunwayAirframeContact(
            Collider aircraftCollider,
            Collider surfaceCollider,
            bool isWheelCollider,
            BoxCollider runwayCollider,
            Vector3 contactPoint)
        {
            return aircraftCollider != null &&
                !isWheelCollider &&
                surfaceCollider is TerrainCollider &&
                !IsInsideRunwayFootprint(runwayCollider, contactPoint);
        }

        public static bool IsInsideRunwayFootprint(BoxCollider runwayCollider, Vector3 point)
        {
            if (runwayCollider == null ||
                !runwayCollider.enabled ||
                !runwayCollider.gameObject.activeInHierarchy ||
                !float.IsFinite(point.x) ||
                !float.IsFinite(point.y) ||
                !float.IsFinite(point.z))
            {
                return false;
            }

            Vector3 localPoint = runwayCollider.transform.InverseTransformPoint(point) -
                runwayCollider.center;
            Vector3 halfSize = runwayCollider.size * 0.5f;
            return Mathf.Abs(localPoint.x) <= halfSize.x &&
                Mathf.Abs(localPoint.z) <= halfSize.z;
        }

        public static bool IsInsideLandingCorridor(
            BoxCollider runwayCollider,
            Vector3 landingDirectionWorld,
            Vector3 contactPoint,
            float distanceWindowMeters,
            float centerlineWindowMeters)
        {
            RunwayApproachSnapshot proximity = RunwayApproachGuidance.CalculateSnapshot(
                contactPoint,
                landingDirectionWorld,
                runwayCollider,
                landingDirectionWorld,
                distanceWindowMeters,
                float.MaxValue);
            return proximity.GuidanceValid &&
                Mathf.Abs(proximity.ThresholdDistanceMeters) <= Mathf.Max(0f, distanceWindowMeters) &&
                Mathf.Abs(proximity.CenterlineErrorMeters) <= Mathf.Max(0f, centerlineWindowMeters);
        }

        public static bool HasStableLandingConditions(
            int groundedWheelCount,
            bool allContactingWheelsOnRunway,
            bool routeCompleted,
            bool targetObserved,
            Vector3 velocity,
            float stableGroundSpeedMps,
            float stableVerticalSpeedMps)
        {
            return routeCompleted &&
                targetObserved &&
                HasStoppedOnRunway(
                    groundedWheelCount,
                    allContactingWheelsOnRunway,
                    velocity,
                    stableGroundSpeedMps,
                    stableVerticalSpeedMps);
        }

        public static bool HasStoppedOnRunway(
            int groundedWheelCount,
            bool allContactingWheelsOnRunway,
            Vector3 velocity,
            float stableGroundSpeedMps,
            float stableVerticalSpeedMps)
        {
            return float.IsFinite(velocity.x) &&
                float.IsFinite(velocity.y) &&
                float.IsFinite(velocity.z) &&
                groundedWheelCount == 3 &&
                allContactingWheelsOnRunway &&
                Vector3.ProjectOnPlane(velocity, Vector3.up).magnitude <= Mathf.Max(0f, stableGroundSpeedMps) &&
                Mathf.Abs(velocity.y) <= Mathf.Max(0f, stableVerticalSpeedMps);
        }

        public static bool HasReachedStableDuration(float stableDurationSeconds, float requiredDurationSeconds)
        {
            return stableDurationSeconds >= Mathf.Max(0f, requiredDurationSeconds);
        }

        private static bool IsOffRunwaySurface(GroundSurfaceType surface)
        {
            return surface == GroundSurfaceType.Terrain || surface == GroundSurfaceType.Other;
        }

        private static bool IsTerminalState(LandingState state)
        {
            return state == LandingState.Successful ||
                state == LandingState.FailedHardLanding ||
                state == LandingState.FailedOffRunway;
        }

        private bool HasValidReferences()
        {
            return _rigidbody != null &&
                _groundController != null &&
                _missionManager != null &&
                _targetObservation != null &&
                _approachGuidance != null &&
                _rigidbody.gameObject == gameObject &&
                _groundController.gameObject == gameObject;
        }

        private void ReportValidationError()
        {
            if (_validationErrorReported)
            {
                return;
            }

            Debug.LogError(
                $"{nameof(AircraftLandingMonitor)} requires serialized Rigidbody, ground controller, mission, target observation and runway guidance references.",
                this);
            _validationErrorReported = true;
        }
    }
}
