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

        private void Awake()
        {
            if (_inputReader == null)
            {
                Debug.LogError(
                    $"{nameof(InputDebugPanel)} requires an {nameof(AircraftInputReader)} reference.",
                    this
                );

                enabled = false;
                return;
            }

            if (_debugText == null)
            {
                Debug.LogError(
                    $"{nameof(InputDebugPanel)} requires a TMP text reference.",
                    this
                );

                enabled = false;
            }
        }

        private void LateUpdate()
        {
            _debugText.text =
                $"INPUT DEBUG\n\n" +
                $"Pitch: {_inputReader.Pitch:F2}\n" +
                $"Roll: {_inputReader.Roll:F2}\n" +
                $"Yaw: {_inputReader.Yaw:F2}\n" +
                $"Throttle: {_inputReader.Throttle:F2}\n" +
                $"Brake: {_inputReader.BrakePressed}\n" +
                $"Camera Switch: {_inputReader.SwitchCameraPressed}\n" +
                $"EO Zoom: {_inputReader.EOZoom:F2}\n" +
                $"Pause: {_inputReader.PausePressed}";
        }
    }
}