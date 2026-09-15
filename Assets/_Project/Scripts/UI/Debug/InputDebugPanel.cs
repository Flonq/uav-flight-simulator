using MertKaan.UAVSimulator.Aircraft;
using MertKaan.UAVSimulator.InputSystem;
using TMPro;
using UnityEngine;

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

            if (_debugText == null)
            {
                Debug.LogError(
                    $"{nameof(InputDebugPanel)} requires a TMP text reference.",
                    this
                );

                enabled = false;
            }
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
                $"Pitch: {_inputReader.Pitch:F2}\n" +
                $"Roll: {_inputReader.Roll:F2}\n" +
                $"Yaw: {_inputReader.Yaw:F2}\n" +
                $"Throttle Input: {_inputReader.ThrottleInput:F2}\n" +
                $"Throttle: {(_engine != null ? _engine.Throttle.ToString("F2") : "N/A")}\n" +
                $"Engine: {(_engine != null ? (!_engine.isActiveAndEnabled ? "Disabled" : (_engine.IsRunning ? "Running" : "Stopped")) : "N/A")}\n" +
                $"RPM: {(_engine != null ? _engine.Rpm.ToString("F0") : "N/A")}\n" +
                $"Thrust: {(_engine != null ? _engine.ThrustNewtons.ToString("F0") : "N/A")} N\n" +
                $"Brake: {_inputReader.BrakePressed}\n" +
                $"Camera Switch: {_inputReader.SwitchCameraPressed}\n" +
                $"EO Zoom: {_inputReader.EOZoom:F2}\n" +
                $"Pause: {_inputReader.PausePressed}";
        }
    }
}
