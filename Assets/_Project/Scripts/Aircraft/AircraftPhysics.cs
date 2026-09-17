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

        public float Airspeed { get; private set; }
        public float ForwardAirspeed { get; private set; }
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

            if (_rigidbody.isKinematic)
            {
                return;
            }

            _rigidbody.AddForce(LiftForce + DragForce, ForceMode.Force);
            _rigidbody.AddTorque(ControlTorque, ForceMode.Force);
        }

        private void CalculateAirData(
            Vector3 aircraftForward,
            Vector3 aircraftUp,
            Vector3 aircraftRight)
        {
            if (Airspeed <= _minimumAerodynamicSpeed)
            {
                ForwardAirspeed = 0f;
                SignedAngleOfAttack = 0f;
                DynamicPressure = 0f;
                LiftCoefficient = 0f;
                DragCoefficient = 0f;
                StallFactor = 0f;
                ControlEffectiveness = 0f;
                LiftForce = Vector3.zero;
                DragForce = Vector3.zero;
                ControlTorque = Vector3.zero;
                return;
            }

            Vector3 localAirVelocity = transform.InverseTransformDirection(AirRelativeVelocity);
            float signedForwardSpeed = -localAirVelocity.z;
            ForwardAirspeed = Mathf.Max(0f, signedForwardSpeed);
            SignedAngleOfAttack = Mathf.Atan2(
                -localAirVelocity.y,
                Mathf.Max(Mathf.Abs(signedForwardSpeed), 0.01f)) * Mathf.Rad2Deg;

            DynamicPressure = 0.5f * _airDensity * Airspeed * Airspeed;
            LiftCoefficient = _liftCoefficientByAngleOfAttack == null
                ? 0f
                : _liftCoefficientByAngleOfAttack.Evaluate(SignedAngleOfAttack);
            StallFactor = Mathf.InverseLerp(
                _stallAngleDegrees,
                _postStallAngleDegrees,
                Mathf.Abs(SignedAngleOfAttack));
            DragCoefficient = _parasiteDragCoefficient +
                _inducedDragFactor * LiftCoefficient * LiftCoefficient +
                _stallDragCoefficient * StallFactor * StallFactor;

            Vector3 airDirection = AirRelativeVelocity / Airspeed;
            Vector3 projectedWingSpan = Vector3.ProjectOnPlane(aircraftRight, airDirection);
            if (projectedWingSpan.sqrMagnitude <= Mathf.Epsilon)
            {
                projectedWingSpan = Vector3.ProjectOnPlane(aircraftUp, airDirection);
            }

            Vector3 liftDirection = Vector3.Cross(
                projectedWingSpan.normalized,
                airDirection).normalized;
            LiftForce = liftDirection * (DynamicPressure * _wingArea * LiftCoefficient);
            DragForce = -airDirection * (DynamicPressure * _wingArea * DragCoefficient);

            float speedAuthority = Mathf.InverseLerp(
                _minimumControlSpeed,
                _fullControlSpeed,
                Airspeed);
            float forwardAlignment = Mathf.Abs(Vector3.Dot(airDirection, aircraftForward));
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
            if (_rigidbody.inertiaTensor.sqrMagnitude <= Mathf.Epsilon)
            {
                return Vector3.zero;
            }

            Quaternion inertiaRotation = _rigidbody.inertiaTensorRotation;
            Vector3 inertiaSpaceAcceleration = inertiaRotation * localAngularAcceleration;
            Vector3 inertiaSpaceTorque = Vector3.Scale(
                _rigidbody.inertiaTensor,
                inertiaSpaceAcceleration);
            Vector3 localTorque = Quaternion.Inverse(inertiaRotation) * inertiaSpaceTorque;
            return _rigidbody.rotation * localTorque;
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

        private void OnDisable()
        {
            Airspeed = 0f;
            ForwardAirspeed = 0f;
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
            FlightState = AircraftFlightState.LowSpeed;
        }
    }
}
