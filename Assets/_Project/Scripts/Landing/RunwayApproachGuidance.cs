using System;
using UnityEngine;

namespace MertKaan.UAVSimulator.Landing
{
    public readonly struct RunwayApproachSnapshot
    {
        public bool GuidanceValid { get; }
        public bool ApproachActive { get; }
        public bool IsBeforeThreshold { get; }
        public bool HasPassedThreshold { get; }
        public float ThresholdDistanceMeters { get; }
        public float CenterlineErrorMeters { get; }
        public float HeadingErrorDegrees { get; }
        public float HeightAboveRunwayMeters { get; }
        public Vector3 ThresholdWorldPosition { get; }
        public Vector3 LandingDirectionWorld { get; }

        public RunwayApproachSnapshot(
            bool guidanceValid,
            bool approachActive,
            bool isBeforeThreshold,
            bool hasPassedThreshold,
            float thresholdDistanceMeters,
            float centerlineErrorMeters,
            float headingErrorDegrees,
            float heightAboveRunwayMeters,
            Vector3 thresholdWorldPosition,
            Vector3 landingDirectionWorld)
        {
            GuidanceValid = guidanceValid;
            ApproachActive = approachActive;
            IsBeforeThreshold = isBeforeThreshold;
            HasPassedThreshold = hasPassedThreshold;
            ThresholdDistanceMeters = thresholdDistanceMeters;
            CenterlineErrorMeters = centerlineErrorMeters;
            HeadingErrorDegrees = headingErrorDegrees;
            HeightAboveRunwayMeters = heightAboveRunwayMeters;
            ThresholdWorldPosition = thresholdWorldPosition;
            LandingDirectionWorld = landingDirectionWorld;
        }

        public static RunwayApproachSnapshot Invalid => new RunwayApproachSnapshot(
            false,
            false,
            false,
            false,
            0f,
            0f,
            0f,
            0f,
            Vector3.zero,
            Vector3.zero);
    }

    /// <summary>
    /// Calculates runway-relative approach data from one explicitly serialized
    /// runway collider and landing direction. It never changes aircraft state.
    /// </summary>
    [DefaultExecutionOrder(260)]
    [DisallowMultipleComponent]
    public sealed class RunwayApproachGuidance : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _aircraftRoot;
        [SerializeField] private BoxCollider _runwayCollider;

        [Header("Landing Direction")]
        [SerializeField] private Vector3 _landingDirectionWorld = Vector3.right;
        [SerializeField, Min(1f)] private float _approachDistanceMeters = 600f;
        [SerializeField, Min(1f)] private float _approachHeightWindowMeters = 120f;

        private RunwayApproachSnapshot _currentSnapshot = RunwayApproachSnapshot.Invalid;
        private bool _validationErrorReported;

        public RunwayApproachSnapshot CurrentSnapshot => _currentSnapshot;
        public BoxCollider RunwayCollider => _runwayCollider;
        public Vector3 LandingDirectionWorld => _landingDirectionWorld;
        public float ApproachDistanceMeters => _approachDistanceMeters;
        public event Action<RunwayApproachSnapshot> SnapshotUpdated;

        private void Reset()
        {
            _aircraftRoot = GameObject.Find("AircraftRoot")?.transform;
        }

        private void OnValidate()
        {
            _approachDistanceMeters = Mathf.Max(1f, _approachDistanceMeters);
            _approachHeightWindowMeters = Mathf.Max(1f, _approachHeightWindowMeters);
        }

        private void Awake()
        {
            if (!HasValidReferences())
            {
                ReportValidationError();
                enabled = false;
            }
        }

        private void FixedUpdate()
        {
            if (!HasValidReferences())
            {
                ReportValidationError();
                return;
            }

            _currentSnapshot = CalculateSnapshot(
                _aircraftRoot.position,
                _aircraftRoot.rotation * Vector3.back,
                _runwayCollider,
                _landingDirectionWorld,
                _approachDistanceMeters,
                _approachHeightWindowMeters);
            SnapshotUpdated?.Invoke(_currentSnapshot);
        }

        public static RunwayApproachSnapshot CalculateSnapshot(
            Vector3 aircraftPosition,
            Vector3 physicalForward,
            BoxCollider runwayCollider,
            Vector3 landingDirectionWorld,
            float approachDistanceMeters,
            float approachHeightWindowMeters)
        {
            if (runwayCollider == null ||
                !TryGetRunwayGeometry(
                    runwayCollider,
                    landingDirectionWorld,
                    out Vector3 direction,
                    out Vector3 runwayCenter,
                    out Vector3 threshold,
                    out float halfLength))
            {
                return RunwayApproachSnapshot.Invalid;
            }

            Vector3 horizontalForward = Vector3.ProjectOnPlane(physicalForward, Vector3.up);
            if (!IsFiniteVector(horizontalForward) || horizontalForward.sqrMagnitude <= Mathf.Epsilon)
            {
                return RunwayApproachSnapshot.Invalid;
            }

            horizontalForward.Normalize();
            float actualHeading = Mathf.Atan2(horizontalForward.x, horizontalForward.z) * Mathf.Rad2Deg;
            float desiredHeading = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float headingError = Mathf.DeltaAngle(actualHeading, desiredHeading);
            float alongFromThreshold = Vector3.Dot(aircraftPosition - threshold, direction);
            float thresholdDistance = -alongFromThreshold;
            Vector3 centerlineAxis = Vector3.Cross(direction, Vector3.up).normalized;
            float centerlineError = Vector3.Dot(aircraftPosition - runwayCenter, centerlineAxis);
            float heightAboveRunway = aircraftPosition.y - runwayCollider.bounds.max.y;
            bool beforeThreshold = alongFromThreshold < 0f;
            bool passedThreshold = !beforeThreshold;
            bool approachActive = beforeThreshold &&
                thresholdDistance <= Mathf.Max(0f, approachDistanceMeters) &&
                Mathf.Abs(heightAboveRunway) <= Mathf.Max(0f, approachHeightWindowMeters);

            return new RunwayApproachSnapshot(
                true,
                approachActive,
                beforeThreshold,
                passedThreshold,
                SanitizeFinite(thresholdDistance),
                SanitizeFinite(centerlineError),
                SanitizeFinite(headingError),
                SanitizeFinite(heightAboveRunway),
                threshold,
                direction);
        }

        public static float CalculateHeadingErrorDegrees(Vector3 physicalForward, Vector3 landingDirectionWorld)
        {
            Vector3 forward = Vector3.ProjectOnPlane(physicalForward, Vector3.up);
            Vector3 direction = Vector3.ProjectOnPlane(landingDirectionWorld, Vector3.up);
            if (forward.sqrMagnitude <= Mathf.Epsilon || direction.sqrMagnitude <= Mathf.Epsilon)
            {
                return 0f;
            }

            forward.Normalize();
            direction.Normalize();
            float actual = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            float desired = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            return Mathf.DeltaAngle(actual, desired);
        }

        private static bool TryGetRunwayGeometry(
            BoxCollider runwayCollider,
            Vector3 landingDirectionWorld,
            out Vector3 direction,
            out Vector3 runwayCenter,
            out Vector3 threshold,
            out float halfLength)
        {
            direction = Vector3.ProjectOnPlane(landingDirectionWorld, Vector3.up);
            runwayCenter = runwayCollider.bounds.center;
            threshold = runwayCenter;
            halfLength = 0f;
            if (!IsFiniteVector(direction) || direction.sqrMagnitude <= Mathf.Epsilon)
            {
                return false;
            }

            direction.Normalize();
            Vector3 axisX = runwayCollider.transform.TransformDirection(Vector3.right).normalized;
            Vector3 axisZ = runwayCollider.transform.TransformDirection(Vector3.forward).normalized;
            Vector3 scale = runwayCollider.transform.lossyScale;
            float extentX = Mathf.Abs(runwayCollider.size.x * scale.x) * 0.5f;
            float extentZ = Mathf.Abs(runwayCollider.size.z * scale.z) * 0.5f;
            Vector3 longAxis = extentX >= extentZ ? axisX : axisZ;
            float longExtent = Mathf.Max(extentX, extentZ);
            if (longExtent <= Mathf.Epsilon || Mathf.Abs(Vector3.Dot(direction, longAxis)) < 0.5f)
            {
                return false;
            }

            halfLength = extentX * Mathf.Abs(Vector3.Dot(direction, axisX)) +
                extentZ * Mathf.Abs(Vector3.Dot(direction, axisZ));
            if (!float.IsFinite(halfLength) || halfLength <= Mathf.Epsilon)
            {
                return false;
            }

            threshold = runwayCenter - direction * halfLength;
            return IsFiniteVector(threshold);
        }

        private bool HasValidReferences()
        {
            return _aircraftRoot != null &&
                _runwayCollider != null &&
                _approachDistanceMeters > 0f &&
                _approachHeightWindowMeters > 0f &&
                Vector3.ProjectOnPlane(_landingDirectionWorld, Vector3.up).sqrMagnitude > Mathf.Epsilon;
        }

        private void ReportValidationError()
        {
            if (_validationErrorReported)
            {
                return;
            }

            Debug.LogError(
                $"{nameof(RunwayApproachGuidance)} requires serialized aircraft root, runway BoxCollider and a valid horizontal landing direction.",
                this);
            _validationErrorReported = true;
        }

        private static bool IsFiniteVector(Vector3 value)
        {
            return float.IsFinite(value.x) && float.IsFinite(value.y) && float.IsFinite(value.z);
        }

        private static float SanitizeFinite(float value)
        {
            return float.IsFinite(value) ? value : 0f;
        }
    }
}
