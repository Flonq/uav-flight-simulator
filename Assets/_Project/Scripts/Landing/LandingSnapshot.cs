using MertKaan.UAVSimulator.Aircraft;
using UnityEngine;

namespace MertKaan.UAVSimulator.Landing
{
    public enum LandingState
    {
        Inactive,
        GroundReady,
        Approach,
        Airborne,
        InitialContact,
        GroundRoll,
        Stopped,
        Successful,
        FailedHardLanding,
        FailedOffRunway
    }

    public enum LandingGearState
    {
        FixedDown
    }

    public enum LandingContactSequence
    {
        None,
        NoseFirst,
        MainGearFirst,
        Simultaneous,
        MainThenNose,
        NoseThenMain
    }

    public readonly struct LandingSnapshot
    {
        public LandingState State { get; }
        public LandingGearState GearState { get; }
        public bool GearReferencesValid { get; }
        public LandingContactSequence ContactSequence { get; }
        public bool HasTouchdown { get; }
        public float TouchdownTimeSeconds { get; }
        public int TouchdownWheelCount { get; }
        public float TouchdownSinkRateMps { get; }
        public Vector3 LastAirborneVelocityMps { get; }
        public GroundSurfaceType FirstNoseWheelSurface { get; }
        public GroundSurfaceType FirstRightMainWheelSurface { get; }
        public GroundSurfaceType FirstLeftMainWheelSurface { get; }
        public int CurrentGroundedWheelCount { get; }
        public GroundSurfaceType CurrentNoseWheelSurface { get; }
        public GroundSurfaceType CurrentRightMainWheelSurface { get; }
        public GroundSurfaceType CurrentLeftMainWheelSurface { get; }
        public bool HasBounced { get; }
        public int BounceCount { get; }
        public float StableDurationSeconds { get; }
        public bool SinkRateWarning { get; }
        public bool RouteCompleted { get; }
        public bool TargetObserved { get; }
        public bool HardLanding { get; }
        public bool OffRunway { get; }
        public bool LandingSuccessful { get; }

        public LandingSnapshot(
            LandingState state,
            LandingGearState gearState,
            bool gearReferencesValid,
            LandingContactSequence contactSequence,
            bool hasTouchdown,
            float touchdownTimeSeconds,
            int touchdownWheelCount,
            float touchdownSinkRateMps,
            Vector3 lastAirborneVelocityMps,
            GroundSurfaceType firstNoseWheelSurface,
            GroundSurfaceType firstRightMainWheelSurface,
            GroundSurfaceType firstLeftMainWheelSurface,
            int currentGroundedWheelCount,
            GroundSurfaceType currentNoseWheelSurface,
            GroundSurfaceType currentRightMainWheelSurface,
            GroundSurfaceType currentLeftMainWheelSurface,
            bool hasBounced,
            int bounceCount,
            float stableDurationSeconds,
            bool sinkRateWarning,
            bool routeCompleted,
            bool targetObserved,
            bool hardLanding,
            bool offRunway,
            bool landingSuccessful)
        {
            State = state;
            GearState = gearState;
            GearReferencesValid = gearReferencesValid;
            ContactSequence = contactSequence;
            HasTouchdown = hasTouchdown;
            TouchdownTimeSeconds = touchdownTimeSeconds;
            TouchdownWheelCount = touchdownWheelCount;
            TouchdownSinkRateMps = touchdownSinkRateMps;
            LastAirborneVelocityMps = lastAirborneVelocityMps;
            FirstNoseWheelSurface = firstNoseWheelSurface;
            FirstRightMainWheelSurface = firstRightMainWheelSurface;
            FirstLeftMainWheelSurface = firstLeftMainWheelSurface;
            CurrentGroundedWheelCount = currentGroundedWheelCount;
            CurrentNoseWheelSurface = currentNoseWheelSurface;
            CurrentRightMainWheelSurface = currentRightMainWheelSurface;
            CurrentLeftMainWheelSurface = currentLeftMainWheelSurface;
            HasBounced = hasBounced;
            BounceCount = bounceCount;
            StableDurationSeconds = stableDurationSeconds;
            SinkRateWarning = sinkRateWarning;
            RouteCompleted = routeCompleted;
            TargetObserved = targetObserved;
            HardLanding = hardLanding;
            OffRunway = offRunway;
            LandingSuccessful = landingSuccessful;
        }

        public static LandingSnapshot Initial => new LandingSnapshot(
            LandingState.Inactive,
            LandingGearState.FixedDown,
            false,
            LandingContactSequence.None,
            false,
            0f,
            0,
            0f,
            Vector3.zero,
            GroundSurfaceType.None,
            GroundSurfaceType.None,
            GroundSurfaceType.None,
            0,
            GroundSurfaceType.None,
            GroundSurfaceType.None,
            GroundSurfaceType.None,
            false,
            0,
            0f,
            false,
            false,
            false,
            false,
            false,
            false);
    }
}
