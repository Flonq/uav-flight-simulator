using MertKaan.UAVSimulator.Aircraft;
using MertKaan.UAVSimulator.CameraSystem;
using MertKaan.UAVSimulator.Landing;
using MertKaan.UAVSimulator.Missions;
using MertKaan.UAVSimulator.Targeting;
using MertKaan.UAVSimulator.Telemetry;

namespace MertKaan.UAVSimulator.UI.Production
{
    public enum ProductionHudAlertLevel
    {
        Normal,
        Caution,
        Danger
    }

    public static class ProductionHudFormatting
    {
        public static string GetFlightStateLabel(AircraftFlightState state)
        {
            switch (state)
            {
                case AircraftFlightState.Grounded:
                    return "Grounded";
                case AircraftFlightState.LowSpeed:
                    return "Low Speed";
                case AircraftFlightState.Flying:
                    return "Flying";
                case AircraftFlightState.Stall:
                    return "Stall";
                case AircraftFlightState.PostStall:
                    return "Post-Stall";
                default:
                    return "Unknown";
            }
        }

        public static string GetSpeedStateLabel(AircraftSpeedState state)
        {
            switch (state)
            {
                case AircraftSpeedState.Normal:
                    return "Normal";
                case AircraftSpeedState.Caution:
                    return "Caution";
                case AircraftSpeedState.Overspeed:
                    return "Overspeed";
                default:
                    return "Unknown";
            }
        }

        public static string GetEngineStateLabel(bool engineRunning)
        {
            return engineRunning ? "Running" : "Stopped";
        }

        public static string GetCameraModeLabel(CameraMode mode)
        {
            return mode == CameraMode.EO ? "EO" : "Chase";
        }

        public static string GetTargetingStateLabel(TargetingState state)
        {
            switch (state)
            {
                case TargetingState.Candidate:
                    return "Candidate";
                case TargetingState.Locked:
                    return "Locked";
                case TargetingState.Observed:
                    return "Observed";
                default:
                    return "No Target";
            }
        }

        public static string GetMissionStateLabel(MissionState state)
        {
            switch (state)
            {
                case MissionState.Active:
                    return "Active";
                case MissionState.RouteCompleted:
                    return "Route Completed";
                default:
                    return "Not Started";
            }
        }

        public static string GetLandingStateLabel(LandingState state)
        {
            switch (state)
            {
                case LandingState.GroundReady:
                    return "ON GROUND";
                case LandingState.Approach:
                    return "APPROACH";
                case LandingState.Airborne:
                    return "AIRBORNE";
                case LandingState.InitialContact:
                    return "CONTACT";
                case LandingState.GroundRoll:
                    return "GROUND ROLL";
                case LandingState.Stopped:
                    return "STOPPED";
                case LandingState.Successful:
                    return "SUCCESSFUL";
                case LandingState.FailedHardLanding:
                    return "HARD LANDING";
                case LandingState.FailedOffRunway:
                    return "OFF RUNWAY";
                default:
                    return "INACTIVE";
            }
        }

        public static string GetLandingGearStateLabel(LandingGearState state)
        {
            return state == LandingGearState.FixedDown ? "FIXED DOWN" : "UNKNOWN";
        }

        public static string GetSinkRateLabel(bool warning)
        {
            return warning ? "SINK WARNING" : "SINK OK";
        }

        public static ProductionHudAlertLevel GetAlertLevel(AircraftTelemetrySnapshot snapshot)
        {
            if (snapshot.FlightState == AircraftFlightState.Stall ||
                snapshot.FlightState == AircraftFlightState.PostStall ||
                snapshot.SpeedState == AircraftSpeedState.Overspeed)
            {
                return ProductionHudAlertLevel.Danger;
            }

            if (snapshot.FlightState == AircraftFlightState.LowSpeed ||
                snapshot.SpeedState == AircraftSpeedState.Caution)
            {
                return ProductionHudAlertLevel.Caution;
            }

            return ProductionHudAlertLevel.Normal;
        }

        public static string GetAlertLabel(AircraftTelemetrySnapshot snapshot)
        {
            if (snapshot.FlightState == AircraftFlightState.Stall ||
                snapshot.FlightState == AircraftFlightState.PostStall)
            {
                return "STALL";
            }

            if (snapshot.SpeedState == AircraftSpeedState.Overspeed)
            {
                return "OVERSPEED";
            }

            if (snapshot.FlightState == AircraftFlightState.LowSpeed)
            {
                return "LOW SPEED";
            }

            if (snapshot.SpeedState == AircraftSpeedState.Caution)
            {
                return "CAUTION";
            }

            return "NORMAL";
        }
    }
}
