using System;
using UnityEngine;

namespace MertKaan.UAVSimulator.InputSystem
{
    public sealed class AircraftInputReader : MonoBehaviour
    {
        public float Pitch { get; private set; }
        public float Roll { get; private set; }
        public float Yaw { get; private set; }

        public float ThrottleInput { get; private set; }

        public bool BrakePressed { get; private set; }
        public bool ToggleEnginePressed { get; private set; }

        public bool SwitchCameraPressed { get; private set; }
        public float EOZoom { get; private set; }

        public bool PausePressed { get; private set; }

        public event Action EngineToggleRequested;

        private AircraftInputActions _inputActions;

        private void Awake()
        {
            _inputActions = new AircraftInputActions();
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
            ThrottleInput = 0f;
            ToggleEnginePressed = false;
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

            ThrottleInput =
                _inputActions.Aircraft.Throttle.ReadValue<float>();

            BrakePressed = _inputActions.Aircraft.Brake.IsPressed();
            ToggleEnginePressed =
                _inputActions.Aircraft.ToggleEngine.WasPressedThisFrame();

            if (ToggleEnginePressed)
            {
                EngineToggleRequested?.Invoke();
            }
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
