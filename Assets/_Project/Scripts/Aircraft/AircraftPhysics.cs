using MertKaan.UAVSimulator.InputSystem;
using UnityEngine;

namespace MertKaan.UAVSimulator.Aircraft
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AircraftInputReader))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class AircraftPhysics : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private AircraftInputReader _inputReader;

        [SerializeField]
        private Rigidbody _rigidbody;

        [Header("Aerodynamics")]
        [SerializeField, Min(0f)]
        private float _airDensity = 1.225f;

        [SerializeField, Min(0.01f)]
        private float _wingArea = 10f;

        [SerializeField, Min(0f)]
        private float _liftCoefficient = 1f;

        [SerializeField, Min(0f)]
        private float _dragCoefficient = 0.08f;

        [Header("Control Authority")]
        [SerializeField, Min(0f)]
        private float _minimumControlSpeed = 5f;

        [SerializeField, Min(0.1f)]
        private float _fullControlSpeed = 15f;

        [SerializeField, Min(0f)]
        private float _maxPitchTorque = 250f;

        [SerializeField, Min(0f)]
        private float _maxRollTorque = 500f;

        [SerializeField, Min(0f)]
        private float _maxYawTorque = 300f;

        public float Airspeed { get; private set; }
        public float ForwardAirspeed { get; private set; }
        public float DynamicPressure { get; private set; }
        public float ControlEffectiveness { get; private set; }
        public Vector3 LiftForce { get; private set; }
        public Vector3 DragForce { get; private set; }
        public Vector3 ControlTorque { get; private set; }

        private void Reset()
        {
            _inputReader = GetComponent<AircraftInputReader>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnValidate()
        {
            _airDensity = Mathf.Max(0f, _airDensity);
            _wingArea = Mathf.Max(0.01f, _wingArea);
            _liftCoefficient = Mathf.Max(0f, _liftCoefficient);
            _dragCoefficient = Mathf.Max(0f, _dragCoefficient);
            _minimumControlSpeed = Mathf.Max(0f, _minimumControlSpeed);
            _fullControlSpeed = Mathf.Max(_minimumControlSpeed + 0.1f, _fullControlSpeed);
            _maxPitchTorque = Mathf.Max(0f, _maxPitchTorque);
            _maxRollTorque = Mathf.Max(0f, _maxRollTorque);
            _maxYawTorque = Mathf.Max(0f, _maxYawTorque);
        }

        private void Awake()
        {
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
            Vector3 aircraftForward = _rigidbody.rotation * Vector3.back;
            Vector3 aircraftUp = _rigidbody.rotation * Vector3.up;
            Vector3 aircraftRight = _rigidbody.rotation * Vector3.right;

            Airspeed = velocity.magnitude;
            ForwardAirspeed = Mathf.Max(0f, Vector3.Dot(velocity, aircraftForward));
            DynamicPressure = 0.5f * _airDensity * ForwardAirspeed * ForwardAirspeed;

            LiftForce = aircraftUp * (DynamicPressure * _wingArea * _liftCoefficient);
            DragForce = CalculateDragForce(velocity, Airspeed);

            ControlEffectiveness = Mathf.InverseLerp(
                _minimumControlSpeed,
                _fullControlSpeed,
                ForwardAirspeed);
            ControlTorque = CalculateControlTorque(
                aircraftForward,
                aircraftUp,
                aircraftRight,
                ControlEffectiveness);

            if (_rigidbody.isKinematic)
            {
                return;
            }

            _rigidbody.AddForce(LiftForce + DragForce, ForceMode.Force);
            _rigidbody.AddTorque(ControlTorque, ForceMode.Force);
        }

        private Vector3 CalculateDragForce(Vector3 velocity, float airspeed)
        {
            if (airspeed <= Mathf.Epsilon)
            {
                return Vector3.zero;
            }

            float dynamicPressure = 0.5f * _airDensity * airspeed * airspeed;
            float dragMagnitude = dynamicPressure * _wingArea * _dragCoefficient;
            return -velocity.normalized * dragMagnitude;
        }

        private Vector3 CalculateControlTorque(
            Vector3 aircraftForward,
            Vector3 aircraftUp,
            Vector3 aircraftRight,
            float effectiveness)
        {
            if (_inputReader == null || !_inputReader.isActiveAndEnabled)
            {
                return Vector3.zero;
            }

            Vector3 pitchTorque =
                aircraftRight * (_inputReader.Pitch * _maxPitchTorque);
            Vector3 rollTorque =
                aircraftForward * (_inputReader.Roll * _maxRollTorque);
            Vector3 yawTorque =
                -aircraftUp * (_inputReader.Yaw * _maxYawTorque);

            return (pitchTorque + rollTorque + yawTorque) * effectiveness;
        }

        private void OnDisable()
        {
            Airspeed = 0f;
            ForwardAirspeed = 0f;
            DynamicPressure = 0f;
            ControlEffectiveness = 0f;
            LiftForce = Vector3.zero;
            DragForce = Vector3.zero;
            ControlTorque = Vector3.zero;
        }
    }
}
