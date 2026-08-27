using UnityEngine;

namespace MertKaan.UAVSimulator.InputSystem
{
    public sealed class AircraftInputReader : MonoBehaviour
    {
        [Header("Throttle")]
        [SerializeField, Min(0f)]
        private float _throttleChangeRate = 0.5f;

        [SerializeField, Range(0f, 1f)]
        private float _initialThrottle = 0f;

        public float Pitch { get; private set; }
        public float Roll { get; private set; }
        public float Yaw { get; private set; }

        public float Throttle { get; private set; }

        public bool BrakePressed { get; private set; }

        public bool SwitchCameraPressed { get; private set; }
        public float EOZoom { get; private set; }

        public bool PausePressed { get; private set; }

        private AircraftInputActions _inputActions;

        private void Awake()
        {
            _inputActions = new AircraftInputActions();

            Throttle = _initialThrottle;
        }

        private void OnEnable()
        {
            _inputActions.Aircraft.Enable();
            _inputActions.Camera.Enable();
            _inputActions.UI.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Aircraft.Disable();
            _inputActions.Camera.Disable();
            _inputActions.UI.Disable();
        }

        private void OnDestroy()
        {
            _inputActions.Dispose();
        }

        private void Update()
        {
            ReadAircraftInput();
            ReadCameraInput();
            ReadUIInput();
        }

        private void ReadAircraftInput()
        {
            Pitch = _inputActions.Aircraft.Pitch.ReadValue<float>();
            Roll = _inputActions.Aircraft.Roll.ReadValue<float>();
            Yaw = _inputActions.Aircraft.Yaw.ReadValue<float>();

            float throttleInput =
                _inputActions.Aircraft.Throttle.ReadValue<float>();

            Throttle = Mathf.Clamp01(
                Throttle + throttleInput * _throttleChangeRate * Time.deltaTime
            );

            BrakePressed = _inputActions.Aircraft.Brake.IsPressed();
        }

        private void ReadCameraInput()
        {
            SwitchCameraPressed =
                _inputActions.Camera.SwitchCamera.WasPressedThisFrame();

            EOZoom =
                _inputActions.Camera.EOZoom.ReadValue<float>();
        }

        private void ReadUIInput()
        {
            PausePressed =
                _inputActions.UI.Pause.WasPressedThisFrame();
        }
    }
}