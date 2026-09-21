using MertKaan.UAVSimulator.Aircraft;
using UnityEngine;

namespace MertKaan.UAVSimulator.Telemetry
{
    /// <summary>
    /// Read-only flight snapshot for the active aircraft.
    /// Values are refreshed from FixedUpdate so consumers can read one coherent
    /// physics-step sample without owning or changing flight state. Body attitude
    /// values are Unity Euler-derived display telemetry; they are not a replacement
    /// for a singularity-free attitude representation near Euler-angle boundaries.
    /// </summary>
    [DefaultExecutionOrder(100)]
    [DisallowMultipleComponent]
    public sealed class AircraftTelemetry : MonoBehaviour
    {
        private const float HeadingHorizontalEpsilon = 0.000001f;
        private const float HeadingNorthSnapDegrees = 0.05f;

        [Header("Sources")]
        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private AircraftPhysics _aircraftPhysics;

        [SerializeField]
        private AircraftEngine _aircraftEngine;

        /// <summary>Horizontal component of world ground velocity, in m/s.</summary>
        public float HorizontalGroundSpeedMps { get; private set; }

        /// <summary>Total air-relative speed reported by AircraftPhysics, in m/s.</summary>
        public float AirspeedMps { get; private set; }

        /// <summary>World-space Y position of the aircraft, in metres.</summary>
        public float AltitudeMeters { get; private set; }

        /// <summary>World vertical velocity, positive upward, in m/s.</summary>
        public float VerticalSpeedMps { get; private set; }

        /// <summary>
        /// Aircraft nose heading in the horizontal world plane, in [0, 360) degrees.
        /// The convention is Unity world +Z = 0 degrees and +X = 90 degrees.
        /// The aircraft's physical forward axis is its local -Z axis. Values within
        /// 0.05 degrees of north are normalized to exactly 0 for stable user-facing
        /// formatting and to avoid a displayed 360-degree boundary value.
        /// </summary>
        public float HeadingDegrees { get; private set; }

        /// <summary>
        /// True when the horizontal projection of the aircraft nose is defined.
        /// When false, HeadingDegrees is the deterministic sentinel value 0.
        /// </summary>
        public bool HeadingValid { get; private set; }

        /// <summary>
        /// Signed body pitch angle from Unity's Euler representation, in degrees,
        /// normalized to [-180, 180]. Euler-angle ambiguity near singularities is
        /// intentionally not replaced by a custom attitude system here.
        /// </summary>
        public float PitchDegrees { get; private set; }

        /// <summary>
        /// Signed body roll angle from Unity's Euler representation, in degrees,
        /// normalized to [-180, 180]. Euler-angle ambiguity near singularities is
        /// intentionally not replaced by a custom attitude system here.
        /// </summary>
        public float RollDegrees { get; private set; }

        /// <summary>
        /// Signed body yaw angle from Unity's Euler representation, in degrees,
        /// normalized to [-180, 180]. Euler-angle ambiguity near singularities is
        /// intentionally not replaced by a custom attitude system here.
        /// </summary>
        public float YawDegrees { get; private set; }

        /// <summary>Current engine throttle as a percentage in [0, 100].</summary>
        public float ThrottlePercent { get; private set; }

        /// <summary>Whether the engine currently reports itself as running.</summary>
        public bool EngineRunning { get; private set; }

        private void Reset()
        {
            ResolveReferences();
        }

        private void Awake()
        {
            ResolveReferences();

            if (_rigidbody == null ||
                _aircraftPhysics == null ||
                _aircraftEngine == null ||
                _rigidbody.gameObject != gameObject ||
                _aircraftPhysics.gameObject != gameObject ||
                _aircraftEngine.gameObject != gameObject)
            {
                Debug.LogError(
                    $"{nameof(AircraftTelemetry)} requires the active aircraft root Rigidbody, " +
                    $"{nameof(AircraftPhysics)} and {nameof(AircraftEngine)}.",
                    this);
                enabled = false;
                return;
            }

            RefreshTelemetry();
        }

        private void FixedUpdate()
        {
            RefreshTelemetry();
        }

        private void ResolveReferences()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            if (_aircraftPhysics == null)
            {
                _aircraftPhysics = GetComponent<AircraftPhysics>();
            }

            if (_aircraftEngine == null)
            {
                _aircraftEngine = GetComponent<AircraftEngine>();
            }
        }

        private void RefreshTelemetry()
        {
            if (_rigidbody == null || _aircraftPhysics == null || _aircraftEngine == null)
            {
                return;
            }

            Vector3 velocity = _rigidbody.linearVelocity;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);

            HorizontalGroundSpeedMps = horizontalVelocity.magnitude;
            AirspeedMps = _aircraftPhysics.Airspeed;
            AltitudeMeters = _rigidbody.position.y;
            VerticalSpeedMps = velocity.y;

            Vector3 eulerAngles = _rigidbody.rotation.eulerAngles;
            PitchDegrees = NormalizeSignedDegrees(eulerAngles.x);
            RollDegrees = NormalizeSignedDegrees(eulerAngles.z);
            YawDegrees = NormalizeSignedDegrees(eulerAngles.y);

            UpdateHeading();

            ThrottlePercent = Mathf.Clamp01(_aircraftEngine.Throttle) * 100f;
            EngineRunning = _aircraftEngine.IsRunning;
        }

        private void UpdateHeading()
        {
            Vector3 physicalForward = _rigidbody.rotation * Vector3.back;
            Vector3 horizontalForward = Vector3.ProjectOnPlane(physicalForward, Vector3.up);

            if (horizontalForward.sqrMagnitude <= HeadingHorizontalEpsilon)
            {
                HeadingDegrees = 0f;
                HeadingValid = false;
                return;
            }

            horizontalForward.Normalize();
            float heading = Mathf.Atan2(horizontalForward.x, horizontalForward.z) * Mathf.Rad2Deg;
            HeadingDegrees = NormalizeHeadingDegrees(heading);
            HeadingValid = true;
        }

        private static float NormalizeHeadingDegrees(float heading)
        {
            float normalizedHeading = Mathf.Repeat(heading, 360f);
            if (normalizedHeading <= HeadingNorthSnapDegrees ||
                normalizedHeading >= 360f - HeadingNorthSnapDegrees)
            {
                return 0f;
            }

            return normalizedHeading;
        }

        private static float NormalizeSignedDegrees(float degrees)
        {
            return Mathf.DeltaAngle(0f, degrees);
        }
    }
}
