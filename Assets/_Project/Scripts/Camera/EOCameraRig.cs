using System;
using MertKaan.UAVSimulator.InputSystem;
using UnityEngine;

namespace MertKaan.UAVSimulator.CameraSystem
{
    public readonly struct EOCameraSnapshot
    {
        public float YawDegrees { get; }
        public float PitchDegrees { get; }
        public float FieldOfViewDegrees { get; }
        public Vector3 Position { get; }
        public Vector3 Forward { get; }

        public EOCameraSnapshot(float yawDegrees, float pitchDegrees, float fieldOfViewDegrees, Vector3 position, Vector3 forward)
        {
            YawDegrees = yawDegrees;
            PitchDegrees = pitchDegrees;
            FieldOfViewDegrees = fieldOfViewDegrees;
            Position = position;
            Forward = forward;
        }
    }

    [DefaultExecutionOrder(105)]
    [DisallowMultipleComponent]
    public sealed class EOCameraRig : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _aircraftRoot;
        [SerializeField] private AircraftInputReader _input;
        [SerializeField] private CameraModeController _cameraModeController;
        [SerializeField] private Transform _yawPivot;
        [SerializeField] private Transform _pitchPivot;
        [SerializeField] private Camera _camera;

        [Header("Mount")]
        [SerializeField] private Vector3 _localOffset = new Vector3(0f, 1.2f, -1.5f);

        [Header("Gimbal")]
        [SerializeField, Min(0f)] private float _yawLimitDegrees = 60f;
        [SerializeField] private float _pitchMinimumDegrees = -45f;
        [SerializeField] private float _pitchMaximumDegrees = 30f;
        [SerializeField, Min(0f)] private float _mouseLookSensitivityDegreesPerPixel = 0.10f;
        [SerializeField, Min(0f)] private float _gamepadLookSpeedDegreesPerSecond = 90f;

        [Header("Zoom")]
        [SerializeField, Min(1f)] private float _minimumFieldOfViewDegrees = 20f;
        [SerializeField, Min(1f)] private float _maximumFieldOfViewDegrees = 60f;
        [SerializeField, Min(0f)] private float _mouseWheelFovStepDegrees = 2.5f;
        [SerializeField, Min(0f)] private float _gamepadZoomSpeedDegreesPerSecond = 55f;

        [Header("Targeting")]
        [SerializeField, Min(1f)] private float _raycastRangeMeters = 1000f;
        [SerializeField] private LayerMask _targetLayerMask = ~0;

        private float _yawDegrees;
        private float _pitchDegrees;
        private float _fieldOfViewDegrees;
        private EOCameraSnapshot _currentSnapshot;
        private bool _initialized;

        public Camera Camera => _camera;
        public float RaycastRangeMeters => _raycastRangeMeters;
        public LayerMask TargetLayerMask => _targetLayerMask;
        public EOCameraSnapshot CurrentSnapshot => _currentSnapshot;
        public event Action<EOCameraSnapshot> SnapshotUpdated;

        private void Awake()
        {
            if (!HasValidReferences())
            {
                Debug.LogError(
                    $"{nameof(EOCameraRig)} requires aircraft, input, camera mode, yaw/pitch pivots and camera references.",
                    this);
                enabled = false;
                return;
            }

            _fieldOfViewDegrees = EOCameraMath.ClampFieldOfView(
                _camera.fieldOfView,
                _minimumFieldOfViewDegrees,
                _maximumFieldOfViewDegrees);
            _camera.fieldOfView = _fieldOfViewDegrees;
            _initialized = true;
        }

        private void LateUpdate()
        {
            if (!_initialized)
            {
                return;
            }

            FollowAircraft();

            if (_cameraModeController.Mode == CameraMode.EO)
            {
                ApplyInput();
            }

            ApplyGimbalPose();
            PublishSnapshot();
        }

        public Ray GetCenterRay()
        {
            return _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        }

        public static bool IsWithinRange(float distanceMeters, float rangeMeters)
        {
            return distanceMeters >= 0f && distanceMeters <= Mathf.Max(0f, rangeMeters);
        }

        private void FollowAircraft()
        {
            transform.SetPositionAndRotation(
                _aircraftRoot.TransformPoint(_localOffset),
                _aircraftRoot.rotation);
        }

        private void ApplyInput()
        {
            Vector2 lookDelta = _input.EOLookSource == EOInputSource.Mouse
                ? EOCameraMath.GetMouseLookDelta(_input.EOLook, _mouseLookSensitivityDegreesPerPixel)
                : _input.EOLookSource == EOInputSource.Gamepad
                    ? EOCameraMath.GetGamepadLookDelta(
                        _input.EOLook,
                        _gamepadLookSpeedDegreesPerSecond,
                        Time.unscaledDeltaTime)
                    : Vector2.zero;

            _yawDegrees = EOCameraMath.ClampYaw(_yawDegrees + lookDelta.x, _yawLimitDegrees);
            _pitchDegrees = EOCameraMath.ClampPitch(
                _pitchDegrees + lookDelta.y,
                _pitchMinimumDegrees,
                _pitchMaximumDegrees);

            float fovDelta = _input.EOZoomSource == EOInputSource.Mouse
                ? EOCameraMath.GetMouseWheelFovDelta(_input.EOZoom, _mouseWheelFovStepDegrees)
                : _input.EOZoomSource == EOInputSource.Gamepad
                    ? EOCameraMath.GetGamepadZoomDelta(
                        _input.EOZoom,
                        _gamepadZoomSpeedDegreesPerSecond,
                        Time.unscaledDeltaTime)
                    : 0f;

            if (!Mathf.Approximately(fovDelta, 0f))
            {
                _fieldOfViewDegrees = EOCameraMath.ClampFieldOfView(
                    _fieldOfViewDegrees + fovDelta,
                    _minimumFieldOfViewDegrees,
                    _maximumFieldOfViewDegrees);
            }
        }

        private void ApplyGimbalPose()
        {
            _yawPivot.localRotation = Quaternion.Euler(0f, _yawDegrees, 0f);
            _pitchPivot.localRotation = Quaternion.Euler(_pitchDegrees, 0f, 0f);
            _camera.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            _camera.fieldOfView = _fieldOfViewDegrees;
        }

        private void PublishSnapshot()
        {
            _currentSnapshot = new EOCameraSnapshot(
                _yawDegrees,
                _pitchDegrees,
                _fieldOfViewDegrees,
                _camera.transform.position,
                _camera.transform.forward);
            SnapshotUpdated?.Invoke(_currentSnapshot);
        }

        private bool HasValidReferences()
        {
            return _aircraftRoot != null &&
                _input != null &&
                _cameraModeController != null &&
                _yawPivot != null &&
                _pitchPivot != null &&
                _camera != null &&
                _raycastRangeMeters > 0f &&
                _maximumFieldOfViewDegrees >= _minimumFieldOfViewDegrees;
        }
    }
}
