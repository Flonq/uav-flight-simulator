using System;
using System.Collections.Generic;
using MertKaan.UAVSimulator.Aircraft;
using MertKaan.UAVSimulator.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MertKaan.UAVSimulator.UI.Debugging
{
    public sealed class InputDebugPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private AircraftInputReader _inputReader;

        [SerializeField]
        private TMP_Text _debugText;

        private AircraftEngine _engine;
        private AircraftPhysics _aircraftPhysics;
        private AircraftGroundController _groundController;
        private AircraftInputActions _bindingActions;
        private string _pitchBinding;
        private string _rollBinding;
        private string _yawBinding;
        private string _throttleBinding;
        private string _brakeBinding;
        private string _engineToggleBinding;
        private string _cameraBinding;
        private string _zoomBinding;
        private string _pauseBinding;

        private void Awake()
        {
            if (!TryResolveInputReader())
            {
                Debug.LogError(
                    $"{nameof(InputDebugPanel)} requires exactly one active {nameof(AircraftInputReader)}.",
                    this
                );

                enabled = false;
                return;
            }

            _engine = _inputReader.GetComponent<AircraftEngine>();
            _aircraftPhysics = _inputReader.GetComponent<AircraftPhysics>();
            _groundController = _inputReader.GetComponent<AircraftGroundController>();

            if (_debugText == null)
            {
                Debug.LogError(
                    $"{nameof(InputDebugPanel)} requires a TMP text reference.",
                    this
                );

                enabled = false;
                return;
            }

            CacheDesktopBindingLabels();
        }

        private void OnDestroy()
        {
            _bindingActions?.Dispose();
        }

        private bool TryResolveInputReader()
        {
            if (_inputReader != null && _inputReader.isActiveAndEnabled)
            {
                return true;
            }

            AircraftInputReader[] activeReaders =
                FindObjectsByType<AircraftInputReader>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None
                );

            if (activeReaders.Length != 1)
            {
                return false;
            }

            _inputReader = activeReaders[0];
            return true;
        }

        private void LateUpdate()
        {
            _debugText.text =
                $"AIRCRAFT DEBUG\n\n" +
                $"Pitch [{_pitchBinding}]: {_inputReader.Pitch:F2}\n" +
                $"Roll [{_rollBinding}]: {_inputReader.Roll:F2}\n" +
                $"Yaw [{_yawBinding}]: {_inputReader.Yaw:F2}\n" +
                $"Throttle [{_throttleBinding}]: {_inputReader.ThrottleInput:F2}\n" +
                $"Throttle State: {(_engine != null ? _engine.Throttle.ToString("F2") : "N/A")}\n" +
                $"Engine Toggle [{_engineToggleBinding}]: {_inputReader.ToggleEnginePressed}\n" +
                $"Engine: {(_engine != null ? (!_engine.isActiveAndEnabled ? "Disabled" : (_engine.IsRunning ? "Running" : "Stopped")) : "N/A")}\n" +
                $"RPM: {(_engine != null ? _engine.Rpm.ToString("F0") : "N/A")}\n" +
                $"Thrust: {(_engine != null ? _engine.ThrustNewtons.ToString("F0") : "N/A")} N\n" +
                $"Airspeed: {(_aircraftPhysics != null ? _aircraftPhysics.Airspeed.ToString("F1") : "N/A")} m/s\n" +
                $"Forward Airspeed: {(_aircraftPhysics != null ? _aircraftPhysics.ForwardAirspeed.ToString("F1") : "N/A")} m/s\n" +
                $"Lateral Airspeed: {(_aircraftPhysics != null ? _aircraftPhysics.LateralAirspeed.ToString("F1") : "N/A")} m/s\n" +
                $"Sideslip Angle: {(_aircraftPhysics != null ? _aircraftPhysics.SideslipAngle.ToString("F1") : "N/A")}°\n" +
                $"Vertical Speed: {(_aircraftPhysics != null ? _aircraftPhysics.VerticalSpeed.ToString("F1") : "N/A")} m/s\n" +
                $"Angle of Attack: {(_aircraftPhysics != null ? _aircraftPhysics.AngleOfAttack.ToString("F1") : "N/A")}°\n" +
                $"Flight State: {(_aircraftPhysics != null ? _aircraftPhysics.FlightState.ToString() : "N/A")}\n" +
                $"Speed State: {(_aircraftPhysics != null ? _aircraftPhysics.SpeedState.ToString() : "N/A")}\n" +
                $"Overspeed Threshold: {(_aircraftPhysics != null ? _aircraftPhysics.OverspeedEntrySpeed.ToString("F1") : "N/A")} m/s\n" +
                $"Ground: {GetGroundStatus()}\n" +
                $"Brake [{_brakeBinding}]: {_inputReader.BrakePressed}\n" +
                $"Camera [{_cameraBinding}]: {_inputReader.SwitchCameraPressed}\n" +
                $"Zoom [{_zoomBinding}]: {_inputReader.EOZoom:F2}\n" +
                $"Pause [{_pauseBinding}]: {_inputReader.PausePressed}";
        }

        private string GetGroundStatus()
        {
            if (_groundController == null)
            {
                return "N/A";
            }

            if (!_groundController.isActiveAndEnabled)
            {
                return "Disabled";
            }

            return _groundController.IsGrounded
                ? $"Grounded ({_groundController.GroundedWheelCount}/3 wheels)"
                : "Airborne";
        }

        private void CacheDesktopBindingLabels()
        {
            _bindingActions = new AircraftInputActions();
            _pitchBinding = GetDesktopBindingDisplayString(_bindingActions.Aircraft.Pitch);
            _rollBinding = GetDesktopBindingDisplayString(_bindingActions.Aircraft.Roll);
            _yawBinding = GetDesktopBindingDisplayString(_bindingActions.Aircraft.Yaw);
            _throttleBinding = GetDesktopBindingDisplayString(_bindingActions.Aircraft.Throttle);
            _brakeBinding = GetDesktopBindingDisplayString(_bindingActions.Aircraft.Brake);
            _engineToggleBinding = GetDesktopBindingDisplayString(_bindingActions.Aircraft.ToggleEngine);
            _cameraBinding = GetDesktopBindingDisplayString(_bindingActions.Camera.SwitchCamera);
            _zoomBinding = GetDesktopBindingDisplayString(_bindingActions.Camera.EOZoom);
            _pauseBinding = GetDesktopBindingDisplayString(_bindingActions.UI.Pause);
        }

        private static string GetDesktopBindingDisplayString(InputAction action)
        {
            List<string> displayStrings = new List<string>();

            for (int i = 0; i < action.bindings.Count; i++)
            {
                InputBinding binding = action.bindings[i];
                if (binding.isPartOfComposite)
                {
                    continue;
                }

                if (binding.isComposite)
                {
                    if (CompositeUsesDesktopDevice(action, i))
                    {
                        displayStrings.Add(
                            NormalizeBindingDisplayString(action.GetBindingDisplayString(i)));
                    }

                    continue;
                }

                if (IsDesktopBindingPath(binding.effectivePath))
                {
                    displayStrings.Add(
                        NormalizeBindingDisplayString(action.GetBindingDisplayString(i)));
                }
            }

            return displayStrings.Count > 0 ? string.Join(" | ", displayStrings) : "Unbound";
        }

        private static string NormalizeBindingDisplayString(string displayString)
        {
            if (displayString.Length != 1)
            {
                return displayString;
            }

            return displayString[0] == '\u0131' ? "I" : displayString.ToUpperInvariant();
        }

        private static bool CompositeUsesDesktopDevice(InputAction action, int compositeIndex)
        {
            for (int i = compositeIndex + 1;
                 i < action.bindings.Count && action.bindings[i].isPartOfComposite;
                 i++)
            {
                if (IsDesktopBindingPath(action.bindings[i].effectivePath))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsDesktopBindingPath(string path)
        {
            return !string.IsNullOrEmpty(path) &&
                   (path.StartsWith("<Keyboard>", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("<Mouse>", StringComparison.OrdinalIgnoreCase));
        }
    }
}
