using System;
using MertKaan.UAVSimulator.InputSystem;
using UnityEngine;

namespace MertKaan.UAVSimulator.CameraSystem
{
    [DefaultExecutionOrder(100)]
    [DisallowMultipleComponent]
    public sealed class CameraModeController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AircraftInputReader _input;
        [SerializeField] private AircraftFollowCamera _chaseCamera;
        [SerializeField] private Camera _chaseCameraComponent;
        [SerializeField] private Camera _eoCamera;
        [SerializeField] private AudioListener _audioListener;

        [Header("Initial State")]
        [SerializeField] private CameraMode _initialMode = CameraMode.Chase;

        private CameraModeSnapshot _currentSnapshot;
        private bool _initialized;

        public CameraMode Mode => _currentSnapshot.Mode;
        public CameraModeSnapshot CurrentSnapshot => _currentSnapshot;
        public Camera ActiveCamera => Mode == CameraMode.EO ? _eoCamera : _chaseCameraComponent;
        public event Action<CameraModeSnapshot> SnapshotUpdated;

        private void Awake()
        {
            if (!HasValidReferences())
            {
                Debug.LogError(
                    $"{nameof(CameraModeController)} requires input, chase camera, EO camera and chase camera component references.",
                    this);
                enabled = false;
                return;
            }

            _currentSnapshot = new CameraModeSnapshot(_initialMode);
            _initialized = true;
        }

        private void OnEnable()
        {
            if (_initialized)
            {
                ApplyMode(_currentSnapshot.Mode, true);
            }
        }

        private void Update()
        {
            if (_input.SwitchCameraPressed)
            {
                SetMode(GetNextMode(Mode));
            }
        }

        public bool SetMode(CameraMode mode)
        {
            if (!_initialized || !HasValidReferences() || mode == Mode)
            {
                return false;
            }

            ApplyMode(mode, false);
            return true;
        }

        public void ToggleMode()
        {
            SetMode(GetNextMode(Mode));
        }

        public static CameraMode GetNextMode(CameraMode mode)
        {
            return mode == CameraMode.Chase ? CameraMode.EO : CameraMode.Chase;
        }

        private void ApplyMode(CameraMode mode, bool force)
        {
            if (!force && mode == Mode)
            {
                return;
            }

            _chaseCameraComponent.enabled = false;
            _eoCamera.enabled = false;

            if (mode == CameraMode.Chase)
            {
                _chaseCamera.enabled = true;
                _chaseCamera.SetTarget(_chaseCamera.Target, true);
                _chaseCameraComponent.enabled = true;
            }
            else
            {
                _chaseCamera.enabled = false;
                _eoCamera.enabled = true;
            }

            if (_audioListener != null)
            {
                _audioListener.enabled = true;
            }

            _currentSnapshot = new CameraModeSnapshot(mode);
            SnapshotUpdated?.Invoke(_currentSnapshot);
        }

        private bool HasValidReferences()
        {
            return _input != null &&
                _chaseCamera != null &&
                _chaseCameraComponent != null &&
                _eoCamera != null;
        }
    }
}
