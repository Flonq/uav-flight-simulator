using System;
using UnityEngine.InputSystem;
using UnityEngine;

namespace MertKaan.UAVSimulator.InputSystem
{
    public enum EOInputSource
    {
        None,
        Mouse,
        Gamepad
    }

    public sealed class AircraftInputReader : MonoBehaviour
    {
        public float Pitch { get; private set; }
        public float Roll { get; private set; }
        public float Yaw { get; private set; }

        public float ThrottleInput { get; private set; }

        public bool BrakePressed { get; private set; }
        public bool ToggleEnginePressed { get; private set; }

        public bool SwitchCameraPressed { get; private set; }
        public Vector2 EOLook { get; private set; }
        public EOInputSource EOLookSource { get; private set; }
        public float EOZoom { get; private set; }
        public EOInputSource EOZoomSource { get; private set; }
        public bool TargetLockPressed { get; private set; }

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
            EOLook = Vector2.zero;
            EOLookSource = EOInputSource.None;
            EOZoom = 0f;
            EOZoomSource = EOInputSource.None;
            TargetLockPressed = false;
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

            EOLook = _inputActions.Camera.EOLook.ReadValue<Vector2>();
            EOLookSource = GetEOInputSource(_inputActions.Camera.EOLook.activeControl);
            EOZoom =
                _inputActions.Camera.EOZoom.ReadValue<float>();
            EOZoomSource = GetEOInputSource(_inputActions.Camera.EOZoom.activeControl);
            TargetLockPressed =
                _inputActions.Camera.TargetLock.WasPressedThisFrame();
        }

        private static EOInputSource GetEOInputSource(InputControl control)
        {
            if (control == null)
            {
                return EOInputSource.None;
            }

            if (control.device is Mouse)
            {
                return EOInputSource.Mouse;
            }

            if (control.device is Gamepad)
            {
                return EOInputSource.Gamepad;
            }

            return EOInputSource.None;
        }

        private void ReadUIInput()
        {
            PausePressed =
                _inputActions.UI.Pause.WasPressedThisFrame();
        }
    }
}
