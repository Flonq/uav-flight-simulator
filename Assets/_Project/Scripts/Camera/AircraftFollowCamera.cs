using UnityEngine;

namespace MertKaan.UAVSimulator.CameraSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class AircraftFollowCamera : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform _target;

        [Header("Framing")]
        [SerializeField] private Vector3 _localOffset = new Vector3(0f, 3f, 9f);
        [SerializeField] private Vector3 _lookAtLocalOffset = new Vector3(0f, 0.8f, -0.5f);

        [Header("Smoothing")]
        [Min(0f)]
        [SerializeField] private float _positionSmoothTime = 0.15f;
        [Min(0f)]
        [SerializeField] private float _rotationSharpness = 10f;

        private Vector3 _positionVelocity;

        public Transform Target => _target;

        private void Awake()
        {
            if (_target == null)
            {
                Debug.LogError($"{nameof(AircraftFollowCamera)} requires an aircraft target.", this);
                enabled = false;
                return;
            }

            SnapToTarget();
        }

        private void OnEnable()
        {
            _positionVelocity = Vector3.zero;
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            Vector3 desiredPosition = _target.TransformPoint(_localOffset);
            if (_positionSmoothTime <= 0f)
            {
                transform.position = desiredPosition;
            }
            else
            {
                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    desiredPosition,
                    ref _positionVelocity,
                    _positionSmoothTime);
            }

            RotateTowardsTarget();
        }

        public void SetTarget(Transform target, bool snapImmediately = true)
        {
            _target = target;
            _positionVelocity = Vector3.zero;

            if (snapImmediately && _target != null)
            {
                SnapToTarget();
            }
        }

        [ContextMenu("Snap To Target")]
        public void SnapToTarget()
        {
            if (_target == null)
            {
                return;
            }

            transform.position = _target.TransformPoint(_localOffset);
            RotateTowardsTarget(true);
            _positionVelocity = Vector3.zero;
        }

        private void RotateTowardsTarget(bool snapImmediately = false)
        {
            Vector3 lookDirection = _target.TransformPoint(_lookAtLocalOffset) - transform.position;
            if (lookDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            Quaternion desiredRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            if (snapImmediately || _rotationSharpness <= 0f)
            {
                transform.rotation = desiredRotation;
                return;
            }

            float interpolation = 1f - Mathf.Exp(-_rotationSharpness * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, interpolation);
        }
    }
}
