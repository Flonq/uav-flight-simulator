using MertKaan.UAVSimulator.InputSystem;
using UnityEngine;

namespace MertKaan.UAVSimulator.Aircraft
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AircraftInputReader))]
    public sealed class AircraftEngine : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private AircraftInputReader _inputReader;

        [Header("Throttle")]
        [SerializeField, Min(0f)]
        private float _throttleChangeRate = 0.5f;

        [SerializeField, Range(0f, 1f)]
        private float _initialThrottle;

        public float Throttle { get; private set; }

        private void Reset()
        {
            _inputReader = GetComponent<AircraftInputReader>();
        }

        private void Awake()
        {
            Throttle = Mathf.Clamp01(_initialThrottle);
            if (_inputReader == null || _inputReader.gameObject != gameObject)
            {
                Debug.LogError($"{nameof(AircraftEngine)} requires its root Input Reader.", this);
                enabled = false;
            }
        }

        private void FixedUpdate()
        {
            if (!_inputReader.isActiveAndEnabled)
            {
                return;
            }

            Throttle = Mathf.Clamp01(
                Throttle + _inputReader.ThrottleInput * _throttleChangeRate * Time.fixedDeltaTime);
        }
    }
}
