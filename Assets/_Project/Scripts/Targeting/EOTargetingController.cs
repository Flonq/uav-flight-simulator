using System;
using MertKaan.UAVSimulator.CameraSystem;
using MertKaan.UAVSimulator.InputSystem;
using UnityEngine;

namespace MertKaan.UAVSimulator.Targeting
{
    [DefaultExecutionOrder(110)]
    [DisallowMultipleComponent]
    public sealed class EOTargetingController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EOCameraRig _eoCameraRig;
        [SerializeField] private CameraModeController _cameraModeController;
        [SerializeField] private AircraftInputReader _input;
        [SerializeField] private EOTarget _missionTarget;

        [Header("Observation")]
        [SerializeField, Min(0.1f)] private float _requiredObservationSeconds = 2f;

        private TargetingSnapshot _currentSnapshot;
        private EOTarget _lockedTarget;
        private float _observationProgressSeconds;
        private bool _initialized;

        public TargetingSnapshot CurrentSnapshot => _currentSnapshot;
        public EOTarget MissionTarget => _missionTarget;
        public float RequiredObservationSeconds => _requiredObservationSeconds;
        public event Action<TargetingSnapshot> SnapshotUpdated;

        private void Awake()
        {
            if (!HasValidReferences())
            {
                Debug.LogError(
                    $"{nameof(EOTargetingController)} requires EO camera rig, camera mode, input and mission target references.",
                    this);
                enabled = false;
                return;
            }

            _initialized = true;
            PublishSnapshot(TargetingState.NoTarget, null, null, false);
        }

        private void LateUpdate()
        {
            if (!_initialized)
            {
                return;
            }

            if (_cameraModeController.Mode != CameraMode.EO)
            {
                ClearLockAndPublish();
                return;
            }

            EOTarget candidate = FindCenterCandidate();
            if (_lockedTarget != null)
            {
                if (candidate != _lockedTarget || !_lockedTarget.IsTargetable)
                {
                    ClearLockAndPublish(candidate);
                    return;
                }

                if (_lockedTarget == _missionTarget)
                {
                    _observationProgressSeconds = Mathf.Min(
                        _requiredObservationSeconds,
                        _observationProgressSeconds + Time.unscaledDeltaTime);
                }

                TargetingState state = _observationProgressSeconds >= _requiredObservationSeconds
                    ? TargetingState.Observed
                    : TargetingState.Locked;
                PublishSnapshot(state, candidate, _lockedTarget, _lockedTarget == _missionTarget);
                return;
            }

            if (candidate != null && _input.TargetLockPressed)
            {
                if (TryLockCandidate(candidate))
                {
                    return;
                }
            }

            PublishSnapshot(
                candidate == null ? TargetingState.NoTarget : TargetingState.Candidate,
                candidate,
                null,
                candidate == _missionTarget);
        }

        public EOTarget FindCenterCandidate()
        {
            Ray ray = _eoCameraRig.GetCenterRay();
            if (!Physics.Raycast(
                    ray,
                    out RaycastHit hit,
                    _eoCameraRig.RaycastRangeMeters,
                    _eoCameraRig.TargetLayerMask,
                    QueryTriggerInteraction.Ignore))
            {
                return null;
            }

            EOTarget target = EOTarget.FromCollider(hit.collider);
            return target != null && target.IsTargetable ? target : null;
        }

        public bool TryLockCandidate(EOTarget candidate)
        {
            if (_lockedTarget != null || !CanLockCandidate(candidate, _missionTarget))
            {
                return false;
            }

            _lockedTarget = candidate;
            _observationProgressSeconds = 0f;
            PublishSnapshot(TargetingState.Locked, candidate, _lockedTarget, true);
            return true;
        }

        public bool RequestLock()
        {
            return TryLockCandidate(FindCenterCandidate());
        }

        public void ClearLockAndPublish()
        {
            ClearLockAndPublish(null);
        }

        private void ClearLockAndPublish(EOTarget candidate)
        {
            _lockedTarget = null;
            _observationProgressSeconds = 0f;
            PublishSnapshot(
                candidate == null ? TargetingState.NoTarget : TargetingState.Candidate,
                candidate,
                null,
                candidate == _missionTarget);
        }

        public static bool CanLockCandidate(EOTarget candidate, EOTarget missionTarget)
        {
            return candidate != null && candidate.IsTargetable && candidate == missionTarget;
        }

        public static float ClampObservationProgress(float progressSeconds, float requiredSeconds)
        {
            return Mathf.Clamp(progressSeconds, 0f, Mathf.Max(0f, requiredSeconds));
        }

        private void PublishSnapshot(TargetingState state, EOTarget candidate, EOTarget lockedTarget, bool isMissionTarget)
        {
            _currentSnapshot = new TargetingSnapshot(
                state,
                candidate,
                lockedTarget,
                ClampObservationProgress(_observationProgressSeconds, _requiredObservationSeconds),
                _requiredObservationSeconds,
                isMissionTarget);
            SnapshotUpdated?.Invoke(_currentSnapshot);
        }

        private bool HasValidReferences()
        {
            return _eoCameraRig != null &&
                _cameraModeController != null &&
                _input != null &&
                _missionTarget != null &&
                _requiredObservationSeconds > 0f;
        }
    }
}
