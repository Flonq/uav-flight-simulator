using MertKaan.UAVSimulator.InputSystem;
using UnityEngine;

namespace MertKaan.UAVSimulator.Aircraft
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AircraftInputReader))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class AircraftEngine : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private AircraftInputReader _inputReader;

        [SerializeField]
        private Rigidbody _rigidbody;

        [Header("Throttle")]
        [SerializeField, Min(0f)]
        private float _throttleChangeRate = 0.5f;

        [SerializeField, Range(0f, 1f)]
        private float _initialThrottle;

        [Header("Motor")]
        [SerializeField]
        private bool _startRunning = true;

        [SerializeField, Min(0f)]
        private float _idleRpm = 1200f;

        [SerializeField, Min(1f)]
        private float _maxRpm = 6000f;

        [SerializeField, Min(1f)]
        private float _rpmChangeRate = 3000f;

        [Header("Propulsion")]
        [SerializeField, Min(0f)]
        private float _maxThrustNewtons = 1000f;

        public float Throttle { get; private set; }
        public bool IsRunning { get; private set; }
        public float Rpm { get; private set; }
        public float NormalizedRpm => Mathf.Clamp01(Rpm / _maxRpm);
        public float ThrustNewtons { get; private set; }

        private void Reset()
        {
            _inputReader = GetComponent<AircraftInputReader>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnValidate()
        {
            _throttleChangeRate = Mathf.Max(0f, _throttleChangeRate);
            _initialThrottle = Mathf.Clamp01(_initialThrottle);
            _maxRpm = Mathf.Max(1f, _maxRpm);
            _idleRpm = Mathf.Clamp(_idleRpm, 0f, _maxRpm - 1f);
            _rpmChangeRate = Mathf.Max(1f, _rpmChangeRate);
            _maxThrustNewtons = Mathf.Max(0f, _maxThrustNewtons);
        }

        private void Awake()
        {
            Throttle = Mathf.Clamp01(_initialThrottle);
            IsRunning = _startRunning;
            if (_inputReader == null || _inputReader.gameObject != gameObject ||
                _rigidbody == null || _rigidbody.gameObject != gameObject)
            {
                Debug.LogError($"{nameof(AircraftEngine)} requires its root Input Reader and Rigidbody.", this);
                enabled = false;
            }
        }

        private void FixedUpdate()
        {
            if (_inputReader.isActiveAndEnabled)
            {
                Throttle = Mathf.Clamp01(
                    Throttle + _inputReader.ThrottleInput * _throttleChangeRate * Time.fixedDeltaTime);
            }

            float targetRpm = IsRunning ? Mathf.Lerp(_idleRpm, _maxRpm, Throttle) : 0f;
            Rpm = Mathf.MoveTowards(Rpm, targetRpm, _rpmChangeRate * Time.fixedDeltaTime);

            float propulsionRpm = Mathf.InverseLerp(_idleRpm, _maxRpm, Rpm);
            ThrustNewtons = IsRunning ? _maxThrustNewtons * propulsionRpm * propulsionRpm : 0f;

            if (ThrustNewtons > 0f && !_rigidbody.isKinematic)
            {
                Vector3 forward = _rigidbody.rotation * Vector3.forward;
                _rigidbody.AddForce(forward * ThrustNewtons, ForceMode.Force);
            }
        }

        public void SetEngineRunning(bool running)
        {
            IsRunning = running;
            if (!running)
            {
                ThrustNewtons = 0f;
            }
        }

        [ContextMenu("Start Engine")]
        private void StartEngine()
        {
            if (Application.isPlaying && isActiveAndEnabled)
            {
                SetEngineRunning(true);
            }
        }

        [ContextMenu("Stop Engine")]
        private void StopEngine()
        {
            if (Application.isPlaying)
            {
                SetEngineRunning(false);
            }
        }

        private void OnDisable()
        {
            Rpm = 0f;
            ThrustNewtons = 0f;
        }
    }
}
