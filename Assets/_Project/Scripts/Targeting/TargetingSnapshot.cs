using UnityEngine;

namespace MertKaan.UAVSimulator.Targeting
{
    public enum TargetingState
    {
        NoTarget = 0,
        Candidate = 1,
        Locked = 2,
        Observed = 3
    }

    public readonly struct TargetingSnapshot
    {
        public TargetingState State { get; }
        public EOTarget Candidate { get; }
        public EOTarget LockedTarget { get; }
        public float ObservationProgressSeconds { get; }
        public float RequiredObservationSeconds { get; }
        public bool IsMissionTarget { get; }

        public bool HasCandidate => Candidate != null;
        public bool HasLock => LockedTarget != null && (State == TargetingState.Locked || State == TargetingState.Observed);
        public bool IsObserved => State == TargetingState.Observed;

        public TargetingSnapshot(
            TargetingState state,
            EOTarget candidate,
            EOTarget lockedTarget,
            float observationProgressSeconds,
            float requiredObservationSeconds,
            bool isMissionTarget)
        {
            State = state;
            Candidate = candidate;
            LockedTarget = lockedTarget;
            ObservationProgressSeconds = observationProgressSeconds;
            RequiredObservationSeconds = requiredObservationSeconds;
            IsMissionTarget = isMissionTarget;
        }
    }
}
