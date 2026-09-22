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

        public MissionObservationSnapshot CurrentSnapshot => _currentSnapshot;
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
            MissionObservationState state = MissionObservationState.NotObserved;
            if (snapshot.IsMissionTarget && snapshot.State == TargetingState.Observed)
            {
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
    }
}
