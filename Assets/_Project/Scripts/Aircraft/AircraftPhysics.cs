using MertKaan.UAVSimulator.InputSystem;
using UnityEngine;

namespace MertKaan.UAVSimulator.Aircraft
{
    public enum AircraftFlightState
    {
        Grounded,
        LowSpeed,
        Flying,
        Stall,
        PostStall
    }

    public enum AircraftSpeedState
    {
        Normal,
        Caution,
        Overspeed
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(AircraftInputReader))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class AircraftPhysics : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private AircraftInputReader _inputReader;

        [SerializeField]
        private AircraftGroundController _groundController;

        [SerializeField]
        private Rigidbody _rigidbody;

        [Header("Air Relative Flow")]
        [Tooltip("World-space wind velocity subtracted from Rigidbody velocity.")]
        [SerializeField]
        private Vector3 _windVelocity;

        [Header("Aerodynamics")]
        [SerializeField, Min(0f)]
        private float _airDensity = 1.225f;

        [SerializeField, Min(0.01f)]
        private float _wingArea = 10f;

        [SerializeField, Min(0.01f)]
        private float _minimumAerodynamicSpeed = 0.5f;

        [Tooltip("Signed lift coefficient sampled by signed angle of attack in degrees.")]
        [SerializeField]
        private AnimationCurve _liftCoefficientByAngleOfAttack = new AnimationCurve(
            new Keyframe(-25f, -0.2f),
            new Keyframe(-15f, -0.55f),
            new Keyframe(-8f, -0.35f),
            new Keyframe(0f, 0.25f),
            new Keyframe(6f, 0.8f),
            new Keyframe(12f, 1.1f),
            new Keyframe(15f, 1.15f),
            new Keyframe(20f, 0.75f),
            new Keyframe(28f, 0.25f),
            new Keyframe(40f, 0f),
            new Keyframe(90f, 0f));

        [SerializeField, Min(0f)]
        private float _parasiteDragCoefficient = 0.035f;

        [SerializeField, Min(0f)]
        private float _inducedDragFactor = 0.045f;

        [SerializeField, Min(0f)]
        private float _stallDragCoefficient = 0.35f;

        [SerializeField, Min(0f)]
        private float _stallAngleDegrees = 14f;

        [SerializeField, Min(0.1f)]
        private float _postStallAngleDegrees = 30f;

        [Header("Lateral Aerodynamics")]
        [Tooltip("Dimensionless side-force coefficient. Positive local +X air-relative velocity produces force toward local -X.")]
        [SerializeField, Range(0f, 1f)]
        private float _sideForceCoefficient = 0.2f;

        [Tooltip("Dimensionless fraction of the side-force moment used for directional stability. Positive sideslip produces a local -Y restoring moment.")]
        [SerializeField, Range(0f, 0.1f)]
        private float _directionalStabilityCoefficient = 0.0015f;

        [Tooltip("Reference length in metres for the directional-stability moment arm.")]
        [SerializeField, Min(0.1f)]
        private float _directionalStabilityReferenceLength = 4f;

        [Tooltip("Dimensionless yaw-rate damping fraction based on the lateral aerodynamic moment scale.")]
        [SerializeField, Range(0f, 0.1f)]
        private float _yawRateDampingCoefficient = 0.02f;

        [Tooltip("Maximum absolute sideslip angle used by the directional-stability coefficient, in degrees.")]
        [SerializeField, Range(5f, 89f)]
        private float _maximumAerodynamicSideslipAngle = 45f;

        [Tooltip("Maximum absolute directional-stability yaw moment, in N m.")]
        [SerializeField, Min(0f)]
        private float _maximumAerodynamicYawMoment = 12f;

        [Header("Control Authority")]
        [SerializeField, Min(0f)]
        private float _minimumControlSpeed = 5f;

        [SerializeField, Min(0.1f)]
        private float _fullControlSpeed = 15f;

        [Range(0f, 1f)]
        [SerializeField]
        private float _minimumResidualControlAuthority = 0.25f;

        [Header("Target Angular Rates")]
        [SerializeField, Min(0f)]
        private float _maxPitchRate = 0.7f;

        [SerializeField, Min(0f)]
        private float _maxRollRate = 1.1f;

        [SerializeField, Min(0f)]
        private float _maxYawRate = 0.55f;

        [Header("Angular Rate Response")]
        [SerializeField, Min(0f)]
        private float _pitchRateResponse = 5f;

        [SerializeField, Min(0f)]
        private float _rollRateResponse = 5f;

        [SerializeField, Min(0f)]
        private float _yawRateResponse = 4f;

        [SerializeField, Min(0f)]
        private float _pitchRateDamping = 2f;

        [SerializeField, Min(0f)]
        private float _rollRateDamping = 2f;

        [SerializeField, Min(0f)]
        private float _yawRateDamping = 1.75f;

        [Header("Angular Acceleration Limits")]
        [SerializeField, Min(0f)]
        private float _maxPitchAngularAcceleration = 4f;

        [SerializeField, Min(0f)]
        private float _maxRollAngularAcceleration = 6f;

        [SerializeField, Min(0f)]
        private float _maxYawAngularAcceleration = 3f;

        [Header("Prototype Speed Envelope")]
        [Tooltip("Total air-relative speed at which the prototype caution band begins, in m/s.")]
        [SerializeField, Min(0.1f)]
        private float _cautionSpeed = 75f;

        [Tooltip("Total air-relative speed at which the prototype overspeed state begins, in m/s. This is a state threshold, not a velocity cap.")]
        [SerializeField, Min(0.1f)]
        private float _overspeedEntrySpeed = 85f;

        [Tooltip("Total air-relative speed at or below which overspeed clears, in m/s. Keep below the overspeed entry threshold to provide hysteresis.")]
        [SerializeField, Min(0.1f)]
        private float _overspeedRecoverySpeed = 80f;

        public float Airspeed { get; private set; }
        public float ForwardAirspeed { get; private set; }
        public float LateralAirspeed { get; private set; }
        public float SideslipAngle { get; private set; }
        public float VerticalSpeed { get; private set; }
        public float SignedAngleOfAttack { get; private set; }
        public float AngleOfAttack => SignedAngleOfAttack;
        public float DynamicPressure { get; private set; }
        public float LiftCoefficient { get; private set; }
        public float DragCoefficient { get; private set; }
        public float StallFactor { get; private set; }
        public float ControlEffectiveness { get; private set; }
        public Vector3 AirRelativeVelocity { get; private set; }
        public Vector3 LiftForce { get; private set; }
        public Vector3 DragForce { get; private set; }
        public Vector3 ControlTorque { get; private set; }
        public AircraftFlightState FlightState { get; private set; }
        public AircraftSpeedState SpeedState { get; private set; }
        public float CautionSpeed => _cautionSpeed;
        public float OverspeedEntrySpeed => _overspeedEntrySpeed;
        public float OverspeedRecoverySpeed => _overspeedRecoverySpeed;

        internal struct LateralAerodynamicState
        {
            public float SignedLateralAirspeed;
            public float SideslipAngle;
            public Vector3 SideForce;
            public float YawMoment;
        }

        private void Reset()
        {
            _inputReader = GetComponent<AircraftInputReader>();
            _groundController = GetComponent<AircraftGroundController>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnValidate()
        {
            _airDensity = Mathf.Max(0f, _airDensity);
            _wingArea = Mathf.Max(0.01f, _wingArea);
            _minimumAerodynamicSpeed = Mathf.Max(0.01f, _minimumAerodynamicSpeed);
            _parasiteDragCoefficient = Mathf.Max(0f, _parasiteDragCoefficient);
            _inducedDragFactor = Mathf.Max(0f, _inducedDragFactor);
            _stallDragCoefficient = Mathf.Max(0f, _stallDragCoefficient);
            _stallAngleDegrees = Mathf.Max(0f, _stallAngleDegrees);
            _postStallAngleDegrees = Mathf.Max(
                _stallAngleDegrees + 0.1f,
                _postStallAngleDegrees);
            _sideForceCoefficient = Mathf.Clamp01(_sideForceCoefficient);
            _directionalStabilityCoefficient = Mathf.Clamp(
                _directionalStabilityCoefficient,
                0f,
                0.1f);
            _directionalStabilityReferenceLength = Mathf.Max(
                0.1f,
                _directionalStabilityReferenceLength);
            _yawRateDampingCoefficient = Mathf.Clamp(
                _yawRateDampingCoefficient,
                0f,
                0.1f);
            _maximumAerodynamicSideslipAngle = Mathf.Clamp(
                _maximumAerodynamicSideslipAngle,
                5f,
                89f);
            _maximumAerodynamicYawMoment = Mathf.Max(
                0f,
                _maximumAerodynamicYawMoment);
            _minimumControlSpeed = Mathf.Max(0f, _minimumControlSpeed);
            _fullControlSpeed = Mathf.Max(_minimumControlSpeed + 0.1f, _fullControlSpeed);
            _minimumResidualControlAuthority = Mathf.Clamp01(_minimumResidualControlAuthority);
            _maxPitchRate = Mathf.Max(0f, _maxPitchRate);
            _maxRollRate = Mathf.Max(0f, _maxRollRate);
            _maxYawRate = Mathf.Max(0f, _maxYawRate);
            _pitchRateResponse = Mathf.Max(0f, _pitchRateResponse);
            _rollRateResponse = Mathf.Max(0f, _rollRateResponse);
            _yawRateResponse = Mathf.Max(0f, _yawRateResponse);
            _pitchRateDamping = Mathf.Max(0f, _pitchRateDamping);
            _rollRateDamping = Mathf.Max(0f, _rollRateDamping);
            _yawRateDamping = Mathf.Max(0f, _yawRateDamping);
            _maxPitchAngularAcceleration = Mathf.Max(0f, _maxPitchAngularAcceleration);
            _maxRollAngularAcceleration = Mathf.Max(0f, _maxRollAngularAcceleration);
            _maxYawAngularAcceleration = Mathf.Max(0f, _maxYawAngularAcceleration);
            _cautionSpeed = Mathf.Max(0.1f, _cautionSpeed);
            _overspeedEntrySpeed = Mathf.Max(
                _cautionSpeed + 0.1f,
                _overspeedEntrySpeed);
            _overspeedRecoverySpeed = Mathf.Clamp(
                _overspeedRecoverySpeed,
                _cautionSpeed,
                _overspeedEntrySpeed - 0.1f);
        }

        private void Awake()
        {
            if (_groundController == null)
            {
                _groundController = GetComponent<AircraftGroundController>();
            }

            if (_inputReader == null ||
                _inputReader.gameObject != gameObject ||
                _rigidbody == null ||
                _rigidbody.gameObject != gameObject)
            {
                Debug.LogError(
                    $"{nameof(AircraftPhysics)} requires its root Input Reader and Rigidbody.",
                    this);
                enabled = false;
            }
        }

        private void FixedUpdate()
        {
            Vector3 velocity = _rigidbody.linearVelocity;
            AirRelativeVelocity = velocity - _windVelocity;
            Airspeed = AirRelativeVelocity.magnitude;
            VerticalSpeed = velocity.y;

            Vector3 aircraftForward = _rigidbody.rotation * Vector3.back;
            Vector3 aircraftUp = _rigidbody.rotation * Vector3.up;
            Vector3 aircraftRight = _rigidbody.rotation * Vector3.right;

            CalculateAirData(aircraftForward, aircraftUp, aircraftRight);
            UpdateFlightState();
            UpdateSpeedState();

            if (_rigidbody.isKinematic)
            {
                return;
            }

            _rigidbody.AddForce(
                LiftForce + DragForce + SideForce,
                ForceMode.Force);
            _rigidbody.AddTorque(
                ControlTorque + DirectionalStabilityTorque,
                ForceMode.Force);
        }

        private Vector3 SideForce { get; set; }
        private Vector3 DirectionalStabilityTorque { get; set; }

        private void CalculateAirData(
            Vector3 aircraftForward,
            Vector3 aircraftUp,
            Vector3 aircraftRight)
        {
            if (Airspeed <= _minimumAerodynamicSpeed)
            {
                ForwardAirspeed = 0f;
                LateralAirspeed = 0f;
                SideslipAngle = 0f;
                SignedAngleOfAttack = 0f;
                DynamicPressure = 0f;
                LiftCoefficient = 0f;
                DragCoefficient = 0f;
                StallFactor = 0f;
                ControlEffectiveness = 0f;
                LiftForce = Vector3.zero;
                DragForce = Vector3.zero;
                ControlTorque = Vector3.zero;
                SideForce = Vector3.zero;
                DirectionalStabilityTorque = Vector3.zero;
                return;
            }

            Vector3 localAirVelocity = transform.InverseTransformDirection(AirRelativeVelocity);
            float signedForwardSpeed = -localAirVelocity.z;
            ForwardAirspeed = Mathf.Max(0f, signedForwardSpeed);
            LateralAirspeed = localAirVelocity.x;
            SideslipAngle = CalculateSideslipAngle(LateralAirspeed, Airspeed);
            SignedAngleOfAttack = signedForwardSpeed > 0f
                ? Mathf.Atan2(-localAirVelocity.y, Mathf.Max(signedForwardSpeed, 0.01f)) * Mathf.Rad2Deg
                : 0f;

            DynamicPressure = 0.5f * _airDensity * Airspeed * Airspeed;
            LiftCoefficient = signedForwardSpeed <= 0f || _liftCoefficientByAngleOfAttack == null
                ? 0f
                : _liftCoefficientByAngleOfAttack.Evaluate(SignedAngleOfAttack);
            StallFactor = Mathf.InverseLerp(
                _stallAngleDegrees,
                _postStallAngleDegrees,
                Mathf.Abs(SignedAngleOfAttack));
            DragCoefficient = _parasiteDragCoefficient +
                _inducedDragFactor * LiftCoefficient * LiftCoefficient +
                _stallDragCoefficient * StallFactor * StallFactor;

            Vector3 longitudinalAirVelocity =
                aircraftForward * signedForwardSpeed +
                aircraftUp * localAirVelocity.y;
            float longitudinalAirspeed = longitudinalAirVelocity.magnitude;
            Vector3 longitudinalAirDirection = longitudinalAirspeed > Mathf.Epsilon
                ? longitudinalAirVelocity / longitudinalAirspeed
                : Vector3.zero;

            Vector3 liftAirVelocity =
                aircraftForward * ForwardAirspeed +
                aircraftUp * localAirVelocity.y;
            float liftAirspeed = liftAirVelocity.magnitude;
            Vector3 liftDirection = Vector3.zero;
            if (signedForwardSpeed > 0f && liftAirspeed > Mathf.Epsilon)
            {
                Vector3 liftAirDirection = liftAirVelocity / liftAirspeed;
                Vector3 projectedWingSpan = Vector3.ProjectOnPlane(
                    aircraftRight,
                    liftAirDirection);
                if (projectedWingSpan.sqrMagnitude <= Mathf.Epsilon)
                {
                    projectedWingSpan = Vector3.ProjectOnPlane(
                        aircraftUp,
                        liftAirDirection);
                }

                if (projectedWingSpan.sqrMagnitude > Mathf.Epsilon)
                {
                    liftDirection = Vector3.Cross(
                        projectedWingSpan.normalized,
                        liftAirDirection).normalized;
                }
            }

            float liftDynamicPressure = 0.5f * _airDensity * liftAirspeed * liftAirspeed;
            LiftForce = liftDirection * (liftDynamicPressure * _wingArea * LiftCoefficient);
            DragForce = longitudinalAirDirection * (
                -0.5f * _airDensity * longitudinalAirspeed * longitudinalAirspeed *
                _wingArea * DragCoefficient);

            LateralAerodynamicState lateralState = CalculateLateralAerodynamics(
                aircraftRight,
                _airDensity,
                _wingArea,
                Airspeed,
                LateralAirspeed,
                transform.InverseTransformDirection(_rigidbody.angularVelocity).y,
                _sideForceCoefficient,
                _directionalStabilityCoefficient,
                _directionalStabilityReferenceLength,
                _yawRateDampingCoefficient,
                _maximumAerodynamicSideslipAngle,
                _maximumAerodynamicYawMoment,
                _minimumAerodynamicSpeed);
            SideForce = lateralState.SideForce;
            DirectionalStabilityTorque = aircraftUp * lateralState.YawMoment;

            float speedAuthority = Mathf.InverseLerp(
                _minimumControlSpeed,
                _fullControlSpeed,
                Airspeed);
            float forwardAlignment = Mathf.Clamp01(
                ForwardAirspeed / Mathf.Max(Airspeed, _minimumAerodynamicSpeed));
            ControlEffectiveness = speedAuthority * Mathf.Lerp(
                _minimumResidualControlAuthority,
                1f,
                forwardAlignment);
            ControlTorque = CalculateControlTorque(ControlEffectiveness);
        }

        private Vector3 CalculateControlTorque(float effectiveness)
        {
            if (_inputReader == null || !_inputReader.isActiveAndEnabled)
            {
                return Vector3.zero;
            }

            Vector3 currentLocalAngularVelocity = transform.InverseTransformDirection(
                _rigidbody.angularVelocity);
            Vector3 targetLocalAngularVelocity = new Vector3(
                _inputReader.Pitch * _maxPitchRate,
                -_inputReader.Yaw * _maxYawRate,
                -_inputReader.Roll * _maxRollRate) * effectiveness;

            Vector3 rateError = targetLocalAngularVelocity - currentLocalAngularVelocity;
            Vector3 localAngularAcceleration = new Vector3(
                CalculateAxisAcceleration(
                    rateError.x,
                    currentLocalAngularVelocity.x,
                    _pitchRateResponse,
                    _pitchRateDamping,
                    _maxPitchAngularAcceleration),
                CalculateAxisAcceleration(
                    rateError.y,
                    currentLocalAngularVelocity.y,
                    _yawRateResponse,
                    _yawRateDamping,
                    _maxYawAngularAcceleration),
                CalculateAxisAcceleration(
                    rateError.z,
                    currentLocalAngularVelocity.z,
                    _rollRateResponse,
                    _rollRateDamping,
                    _maxRollAngularAcceleration));

            return AngularAccelerationToWorldTorque(localAngularAcceleration);
        }

        internal static float CalculateSideslipAngle(
            float signedLateralAirspeed,
            float airspeed)
        {
            if (!IsFinite(signedLateralAirspeed) ||
                !IsFinite(airspeed) ||
                airspeed <= 0f)
            {
                return 0f;
            }

            return Mathf.Asin(Mathf.Clamp(
                signedLateralAirspeed / airspeed,
                -1f,
                1f)) * Mathf.Rad2Deg;
        }

        internal static LateralAerodynamicState CalculateLateralAerodynamics(
            Vector3 aircraftRight,
            float airDensity,
            float wingArea,
            float airspeed,
            float signedLateralAirspeed,
            float localYawRate,
            float sideForceCoefficient,
            float directionalStabilityCoefficient,
            float directionalStabilityReferenceLength,
            float yawRateDampingCoefficient,
            float maximumAerodynamicSideslipAngle,
            float maximumAerodynamicYawMoment,
            float minimumAerodynamicSpeed)
        {
            LateralAerodynamicState state = new LateralAerodynamicState
            {
                SignedLateralAirspeed = signedLateralAirspeed,
                SideslipAngle = CalculateSideslipAngle(
                    signedLateralAirspeed,
                    airspeed),
                SideForce = Vector3.zero,
                YawMoment = 0f
            };

            if (!IsFinite(airDensity) ||
                !IsFinite(wingArea) ||
                !IsFinite(airspeed) ||
                !IsFinite(signedLateralAirspeed) ||
                !IsFinite(localYawRate) ||
                airDensity <= 0f ||
                wingArea <= 0f ||
                airspeed <= Mathf.Max(minimumAerodynamicSpeed, 0f) ||
                aircraftRight.sqrMagnitude <= Mathf.Epsilon)
            {
                state.SignedLateralAirspeed = 0f;
                state.SideslipAngle = 0f;
                return state;
            }

            float safeSideForceCoefficient = Mathf.Clamp01(sideForceCoefficient);
            float safeDirectionalStabilityCoefficient = Mathf.Clamp(
                directionalStabilityCoefficient,
                0f,
                0.1f);
            float safeReferenceLength = Mathf.Max(
                directionalStabilityReferenceLength,
                0.1f);
            float safeYawRateDampingCoefficient = Mathf.Clamp(
                yawRateDampingCoefficient,
                0f,
                0.1f);
            float safeMaximumSideslipAngle = Mathf.Clamp(
                maximumAerodynamicSideslipAngle,
                5f,
                89f);
            float safeMaximumYawMoment = Mathf.Max(
                maximumAerodynamicYawMoment,
                0f);

            Vector3 right = aircraftRight.normalized;
            float lateralVelocityRatio = Mathf.Clamp(
                signedLateralAirspeed / airspeed,
                -1f,
                1f);
            float dynamicPressure = 0.5f * airDensity * airspeed * airspeed;
            float sideForceScalar =
                -dynamicPressure * wingArea * safeSideForceCoefficient * lateralVelocityRatio;
            state.SideForce = right * sideForceScalar;

            float betaRadians = state.SideslipAngle * Mathf.Deg2Rad;
            float betaLimitRadians = safeMaximumSideslipAngle * Mathf.Deg2Rad;
            float betaFactor = Mathf.Clamp(
                betaRadians / betaLimitRadians,
                -1f,
                1f);
            float yawRateFactor = Mathf.Clamp(
                localYawRate * safeReferenceLength / airspeed,
                -1f,
                1f);
            float restoringYawMoment =
                -dynamicPressure * wingArea * safeSideForceCoefficient *
                safeReferenceLength * safeDirectionalStabilityCoefficient * betaFactor;
            float yawRateDampingMoment =
                -dynamicPressure * wingArea * safeSideForceCoefficient *
                safeReferenceLength * safeYawRateDampingCoefficient * yawRateFactor;
            state.YawMoment = Mathf.Clamp(
                restoringYawMoment + yawRateDampingMoment,
                -safeMaximumYawMoment,
                safeMaximumYawMoment);
            return state;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        private static float CalculateAxisAcceleration(
            float rateError,
            float currentRate,
            float response,
            float damping,
            float maximumAcceleration)
        {
            float acceleration = rateError * response - currentRate * damping;
            return Mathf.Clamp(acceleration, -maximumAcceleration, maximumAcceleration);
        }

        private Vector3 AngularAccelerationToWorldTorque(Vector3 localAngularAcceleration)
        {
            return ConvertLocalAngularAccelerationToWorldTorque(
                localAngularAcceleration,
                _rigidbody.inertiaTensor,
                _rigidbody.inertiaTensorRotation,
                _rigidbody.rotation);
        }

        internal static Vector3 ConvertLocalAngularAccelerationToWorldTorque(
            Vector3 localAngularAcceleration,
            Vector3 inertiaTensor,
            Quaternion inertiaTensorRotation,
            Quaternion bodyRotation)
        {
            if (inertiaTensor.sqrMagnitude <= Mathf.Epsilon)
            {
                return Vector3.zero;
            }

            // inertiaTensorRotation maps principal inertia axes into Rigidbody-local space.
            Vector3 inertiaSpaceAcceleration =
                Quaternion.Inverse(inertiaTensorRotation) * localAngularAcceleration;
            Vector3 inertiaSpaceTorque = Vector3.Scale(
                inertiaTensor,
                inertiaSpaceAcceleration);
            Vector3 localTorque = inertiaTensorRotation * inertiaSpaceTorque;
            return bodyRotation * localTorque;
        }

        private void UpdateFlightState()
        {
            if (_groundController != null && _groundController.IsGrounded)
            {
                FlightState = AircraftFlightState.Grounded;
            }
            else if (Airspeed < _minimumControlSpeed)
            {
                FlightState = AircraftFlightState.LowSpeed;
            }
            else if (Mathf.Abs(SignedAngleOfAttack) >= _postStallAngleDegrees)
            {
                FlightState = AircraftFlightState.PostStall;
            }
            else if (Mathf.Abs(SignedAngleOfAttack) >= _stallAngleDegrees)
            {
                FlightState = AircraftFlightState.Stall;
            }
            else
            {
                FlightState = AircraftFlightState.Flying;
            }
        }

        private void UpdateSpeedState()
        {
            SpeedState = EvaluateSpeedState(
                SpeedState,
                Airspeed,
                _cautionSpeed,
                _overspeedEntrySpeed,
                _overspeedRecoverySpeed);
        }

        internal static AircraftSpeedState EvaluateSpeedState(
            AircraftSpeedState currentState,
            float airspeed,
            float cautionSpeed,
            float overspeedEntrySpeed,
            float overspeedRecoverySpeed)
        {
            if (!IsFinite(airspeed) || airspeed < 0f)
            {
                return AircraftSpeedState.Normal;
            }

            float safeCautionSpeed = Mathf.Max(0.1f, cautionSpeed);
            float safeOverspeedEntrySpeed = Mathf.Max(
                safeCautionSpeed + 0.1f,
                overspeedEntrySpeed);
            float safeOverspeedRecoverySpeed = Mathf.Clamp(
                overspeedRecoverySpeed,
                safeCautionSpeed,
                safeOverspeedEntrySpeed - 0.1f);

            if (currentState == AircraftSpeedState.Overspeed)
            {
                if (airspeed <= safeOverspeedRecoverySpeed)
                {
                    return airspeed >= safeCautionSpeed
                        ? AircraftSpeedState.Caution
                        : AircraftSpeedState.Normal;
                }

                return AircraftSpeedState.Overspeed;
            }

            if (airspeed >= safeOverspeedEntrySpeed)
            {
                return AircraftSpeedState.Overspeed;
            }

            return airspeed >= safeCautionSpeed
                ? AircraftSpeedState.Caution
                : AircraftSpeedState.Normal;
        }

        private void OnDisable()
        {
            Airspeed = 0f;
            ForwardAirspeed = 0f;
            LateralAirspeed = 0f;
            SideslipAngle = 0f;
            VerticalSpeed = 0f;
            SignedAngleOfAttack = 0f;
            DynamicPressure = 0f;
            LiftCoefficient = 0f;
            DragCoefficient = 0f;
            StallFactor = 0f;
            ControlEffectiveness = 0f;
            AirRelativeVelocity = Vector3.zero;
            LiftForce = Vector3.zero;
            DragForce = Vector3.zero;
            ControlTorque = Vector3.zero;
            SideForce = Vector3.zero;
            DirectionalStabilityTorque = Vector3.zero;
            FlightState = AircraftFlightState.LowSpeed;
            SpeedState = AircraftSpeedState.Normal;
        }
    }
}
