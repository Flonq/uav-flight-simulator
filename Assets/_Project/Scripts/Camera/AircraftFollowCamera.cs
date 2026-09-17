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

        private const float HeadingEpsilonSqr = 0.0001f;
        private const float DirectionEpsilonSqr = 0.000001f;

        private Vector3 _positionVelocity;
        private Vector3 _lastHeadingForward;
        private Vector3 _lastRight;
        private Vector3 _previousTargetPosition;
        private bool _hasCachedHeading;
        private bool _hasCachedRight;
        private bool _hasPreviousTargetPosition;

        private struct ChaseFrame
        {
            public Vector3 right;
            public Vector3 up;
            public Vector3 chaseOffset;
            public Vector3 lookAtPosition;
        }

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
            ResetTrackingState();
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            ChaseFrame chaseFrame = BuildChaseFrame();
            if (!_hasPreviousTargetPosition)
            {
                _previousTargetPosition = _target.position;
                _hasPreviousTargetPosition = true;
            }

            Vector3 targetDelta = _target.position - _previousTargetPosition;
            Vector3 desiredPosition = _target.position + chaseFrame.chaseOffset;
            Vector3 feedForwardPosition = transform.position + targetDelta;
            if (_positionSmoothTime <= 0f)
            {
                transform.position = desiredPosition;
            }
            else
            {
                transform.position = Vector3.SmoothDamp(
                    feedForwardPosition,
                    desiredPosition,
                    ref _positionVelocity,
                    _positionSmoothTime);
            }

            _previousTargetPosition = _target.position;
            RotateTowardsTarget(chaseFrame);
        }

        public void SetTarget(Transform target, bool snapImmediately = true)
        {
            _target = target;
            ResetTrackingState();

            if (_target != null)
            {
                _previousTargetPosition = _target.position;
                _hasPreviousTargetPosition = true;
            }

            if (snapImmediately && _target != null)
            {
                SnapToTarget();
            }
        }

        [ContextMenu("Snap To Target")]
        public void SnapToTarget()
        {
            ResetTrackingState();

            if (_target == null)
            {
                return;
            }

            ChaseFrame chaseFrame = BuildChaseFrame();
            transform.position = _target.position + chaseFrame.chaseOffset;
            RotateTowardsTarget(chaseFrame, true);
            _previousTargetPosition = _target.position;
            _hasPreviousTargetPosition = true;
        }

        private void ResetTrackingState()
        {
            _positionVelocity = Vector3.zero;
            _lastHeadingForward = Vector3.zero;
            _lastRight = Vector3.zero;
            _previousTargetPosition = Vector3.zero;
            _hasCachedHeading = false;
            _hasCachedRight = false;
            _hasPreviousTargetPosition = false;
        }

        private ChaseFrame BuildChaseFrame()
        {
            Vector3 physicalForward = -_target.forward;
            Vector3 horizontalForward = Vector3.ProjectOnPlane(physicalForward, Vector3.up);
            float horizontalMagnitudeSqr = horizontalForward.sqrMagnitude;

            if (horizontalMagnitudeSqr > HeadingEpsilonSqr)
            {
                _lastHeadingForward = horizontalForward / Mathf.Sqrt(horizontalMagnitudeSqr);
                _hasCachedHeading = true;
            }
            else if (!_hasCachedHeading)
            {
                _lastHeadingForward = Vector3.back;
                _hasCachedHeading = true;
            }

            Vector3 rightCandidate = Vector3.Cross(physicalForward, Vector3.up);
            if (rightCandidate.sqrMagnitude > HeadingEpsilonSqr)
            {
                rightCandidate.Normalize();
                if (_hasCachedRight && Vector3.Dot(rightCandidate, _lastRight) < 0f)
                {
                    rightCandidate = -rightCandidate;
                }

                _lastRight = rightCandidate;
                _hasCachedRight = true;
            }
            else if (!_hasCachedRight)
            {
                _lastRight = Vector3.Cross(_lastHeadingForward, Vector3.up);
                if (_lastRight.sqrMagnitude <= DirectionEpsilonSqr)
                {
                    _lastRight = Vector3.right;
                }
                else
                {
                    _lastRight.Normalize();
                }

                _hasCachedRight = true;
            }

            Vector3 frameUp = Vector3.Cross(_lastRight, physicalForward);
            if (frameUp.sqrMagnitude <= DirectionEpsilonSqr)
            {
                frameUp = Vector3.up;
            }
            else
            {
                frameUp.Normalize();
            }

            return new ChaseFrame
            {
                right = _lastRight,
                up = frameUp,
                chaseOffset =
                    _lastRight * _localOffset.x +
                    frameUp * _localOffset.y -
                    physicalForward * _localOffset.z,
                lookAtPosition = _target.position +
                    _lastRight * _lookAtLocalOffset.x +
                    frameUp * _lookAtLocalOffset.y -
                    physicalForward * _lookAtLocalOffset.z
            };
        }

        private void RotateTowardsTarget(ChaseFrame chaseFrame, bool snapImmediately = false)
        {
            Vector3 lookDirection = chaseFrame.lookAtPosition - transform.position;
            if (lookDirection.sqrMagnitude <= DirectionEpsilonSqr)
            {
                return;
            }

            Quaternion desiredRotation = CalculateLookRotation(lookDirection, chaseFrame.up, chaseFrame.right);
            if (snapImmediately || _rotationSharpness <= 0f)
            {
                transform.rotation = desiredRotation;
                return;
            }

            float interpolation = 1f - Mathf.Exp(-_rotationSharpness * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, interpolation);
        }

        private Quaternion CalculateLookRotation(Vector3 lookDirection, Vector3 frameUp, Vector3 frameRight)
        {
            Vector3 up = frameUp;
            if (Mathf.Abs(Vector3.Dot(lookDirection.normalized, up)) > 0.9999f)
            {
                up = frameRight;
            }

            return Quaternion.LookRotation(lookDirection, up);
        }
    }
}
