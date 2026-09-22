using MertKaan.UAVSimulator.Aircraft;
using MertKaan.UAVSimulator.Missions;
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
