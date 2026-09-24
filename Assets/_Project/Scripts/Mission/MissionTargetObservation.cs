using System;
using MertKaan.UAVSimulator.Targeting;
using UnityEngine;

namespace MertKaan.UAVSimulator.Missions
{
    public enum MissionObservationState
    {
        NotObserved = 0,
        Observing = 1,
        Observed = 2
    }

    public readonly struct MissionObservationSnapshot
    {
        public MissionObservationState State { get; }
        public EOTarget Target { get; }
        public float ProgressSeconds { get; }
        public float RequiredSeconds { get; }

        public bool IsObserved => State == MissionObservationState.Observed;

        public MissionObservationSnapshot(
            MissionObservationState state,
            EOTarget target,
            float progressSeconds,
            float requiredSeconds)
        {
            State = state;
            Target = target;
            ProgressSeconds = progressSeconds;
            RequiredSeconds = requiredSeconds;
        }
    }

    [DefaultExecutionOrder(220)]
    [DisallowMultipleComponent]
    public sealed class MissionTargetObservation : MonoBehaviour
    {
        [SerializeField] private EOTargetingController _targetingController;

        private MissionObservationSnapshot _currentSnapshot;
        private bool _subscribed;
        private bool _observationRecorded;
        private bool _ignoreObservedUntilFreshLock;

        public MissionObservationSnapshot CurrentSnapshot => _currentSnapshot;
        public bool IsObservationRecorded => _observationRecorded;
        public event Action<MissionObservationSnapshot> SnapshotUpdated;

        private void OnEnable()
        {
            if (_targetingController == null)
            {
                Debug.LogError($"{nameof(MissionTargetObservation)} requires a targeting controller reference.", this);
                enabled = false;
                return;
            }

            _targetingController.SnapshotUpdated += HandleTargetingSnapshot;
            _subscribed = true;
            HandleTargetingSnapshot(_targetingController.CurrentSnapshot);
        }

        private void OnDisable()
        {
            if (_subscribed && _targetingController != null)
            {
                _targetingController.SnapshotUpdated -= HandleTargetingSnapshot;
            }

            _subscribed = false;
        }

        private void HandleTargetingSnapshot(TargetingSnapshot snapshot)
        {
            if (_ignoreObservedUntilFreshLock && snapshot.State != TargetingState.Observed)
            {
                _ignoreObservedUntilFreshLock = false;
            }

            MissionObservationState state = MissionObservationState.NotObserved;
            if (_observationRecorded)
            {
                state = MissionObservationState.Observed;
            }
            else if (snapshot.IsMissionTarget &&
                snapshot.State == TargetingState.Observed &&
                !_ignoreObservedUntilFreshLock)
            {
                _observationRecorded = true;
                state = MissionObservationState.Observed;
            }
            else if (snapshot.IsMissionTarget && snapshot.HasLock)
            {
                state = MissionObservationState.Observing;
            }

            _currentSnapshot = new MissionObservationSnapshot(
                state,
                snapshot.LockedTarget ?? snapshot.Candidate,
                snapshot.ObservationProgressSeconds,
                snapshot.RequiredObservationSeconds);
            SnapshotUpdated?.Invoke(_currentSnapshot);
        }

        /// <summary>
        /// Clears the persistent mission record. If the targeting controller is
        /// still in Observed, a fresh lock must first leave that state before a
        /// later observation can be recorded again.
        /// </summary>
        public void ResetObservation()
        {
            _observationRecorded = false;
            _ignoreObservedUntilFreshLock = _targetingController != null &&
                _targetingController.CurrentSnapshot.State == TargetingState.Observed;
            _currentSnapshot = new MissionObservationSnapshot(
                MissionObservationState.NotObserved,
                null,
                0f,
                _targetingController == null ? 0f : _targetingController.RequiredObservationSeconds);
            SnapshotUpdated?.Invoke(_currentSnapshot);
        }
    }
}
