namespace MertKaan.UAVSimulator.Telemetry
{
    /// <summary>
    /// Immutable value snapshot of one AircraftTelemetry physics sample.
    /// All numeric members use the same units and conventions as AircraftTelemetry.
    /// </summary>
    public readonly struct AircraftTelemetrySnapshot
    {
        /// <summary>Horizontal world ground speed, in m/s.</summary>
        public readonly float HorizontalGroundSpeedMps;

        /// <summary>Total air-relative speed, in m/s.</summary>
        public readonly float AirspeedMps;

        /// <summary>World-space Y position, in metres.</summary>
        public readonly float AltitudeMeters;

        /// <summary>World vertical speed, positive upward, in m/s.</summary>
        public readonly float VerticalSpeedMps;

        /// <summary>Physical nose heading in [0, 360) degrees.</summary>
        public readonly float HeadingDegrees;

        /// <summary>Whether the horizontal nose projection defines HeadingDegrees.</summary>
        public readonly bool HeadingValid;

        /// <summary>Signed body pitch angle, in degrees.</summary>
        public readonly float PitchDegrees;

        /// <summary>Signed body roll angle, in degrees.</summary>
        public readonly float RollDegrees;

        /// <summary>Signed body yaw angle, in degrees.</summary>
        public readonly float YawDegrees;

        /// <summary>Engine throttle, in percent [0, 100].</summary>
        public readonly float ThrottlePercent;

        /// <summary>Whether the engine reports itself as running.</summary>
        public readonly bool EngineRunning;

        public AircraftTelemetrySnapshot(
            float horizontalGroundSpeedMps,
            float airspeedMps,
            float altitudeMeters,
            float verticalSpeedMps,
            float headingDegrees,
            bool headingValid,
            float pitchDegrees,
            float rollDegrees,
            float yawDegrees,
            float throttlePercent,
            bool engineRunning)
        {
            HorizontalGroundSpeedMps = horizontalGroundSpeedMps;
            AirspeedMps = airspeedMps;
            AltitudeMeters = altitudeMeters;
            VerticalSpeedMps = verticalSpeedMps;
            HeadingDegrees = headingDegrees;
            HeadingValid = headingValid;
            PitchDegrees = pitchDegrees;
            RollDegrees = rollDegrees;
            YawDegrees = yawDegrees;
            ThrottlePercent = throttlePercent;
            EngineRunning = engineRunning;
        }
    }
}
