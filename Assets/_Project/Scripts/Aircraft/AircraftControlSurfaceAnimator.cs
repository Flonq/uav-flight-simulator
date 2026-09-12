using MertKaan.UAVSimulator.InputSystem;
using UnityEngine;

namespace MertKaan.UAVSimulator.Aircraft
{
    [DisallowMultipleComponent]
    public sealed class AircraftControlSurfaceAnimator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private AircraftInputReader _inputReader;

        [SerializeField]
        private Transform _leftAileron;

        [SerializeField]
        private Transform _rightAileron;

        [SerializeField]
        private Transform _leftRuddervator;

        [SerializeField]
        private Transform _rightRuddervator;

        [Header("Deflection")]
        [SerializeField, Range(0f, 45f)]
        private float _maxAileronAngle = 20f;

        [SerializeField, Range(0f, 45f)]
        private float _maxRuddervatorPitchAngle = 20f;

        [SerializeField, Range(0f, 45f)]
        private float _maxRuddervatorYawAngle = 15f;

        [SerializeField, Range(0f, 45f)]
        private float _maxRuddervatorCombinedAngle = 25f;

        [SerializeField, Min(0f)]
        private float _surfaceSpeed = 90f;

        private Quaternion _leftAileronNeutralRotation;
        private Quaternion _rightAileronNeutralRotation;
        private Quaternion _leftRuddervatorNeutralRotation;
        private Quaternion _rightRuddervatorNeutralRotation;

        private float _leftAileronAngle;
        private float _rightAileronAngle;
        private float _leftRuddervatorAngle;
        private float _rightRuddervatorAngle;
        private bool _isInitialized;

        private void Reset()
        {
            _inputReader = GetComponent<AircraftInputReader>();
        }

        private void Awake()
        {
            if (!TryValidateReferences())
            {
                enabled = false;
                return;
            }

            _leftAileronNeutralRotation = _leftAileron.localRotation;
            _rightAileronNeutralRotation = _rightAileron.localRotation;
            _leftRuddervatorNeutralRotation = _leftRuddervator.localRotation;
            _rightRuddervatorNeutralRotation = _rightRuddervator.localRotation;
            _isInitialized = true;
        }

        private void LateUpdate()
        {
            float step = _surfaceSpeed * Time.deltaTime;

            float leftAileronTarget = -_inputReader.Roll * _maxAileronAngle;
            float rightAileronTarget = _inputReader.Roll * _maxAileronAngle;

            float pitchAngle = _inputReader.Pitch * _maxRuddervatorPitchAngle;
            float yawAngle = _inputReader.Yaw * _maxRuddervatorYawAngle;
            float leftRuddervatorTarget = Mathf.Clamp(
                pitchAngle + yawAngle,
                -_maxRuddervatorCombinedAngle,
                _maxRuddervatorCombinedAngle);
            float rightRuddervatorTarget = Mathf.Clamp(
                pitchAngle - yawAngle,
                -_maxRuddervatorCombinedAngle,
                _maxRuddervatorCombinedAngle);

            _leftAileronAngle = Mathf.MoveTowards(
                _leftAileronAngle,
                leftAileronTarget,
                step);
            _rightAileronAngle = Mathf.MoveTowards(
                _rightAileronAngle,
                rightAileronTarget,
                step);
            _leftRuddervatorAngle = Mathf.MoveTowards(
                _leftRuddervatorAngle,
                leftRuddervatorTarget,
                step);
            _rightRuddervatorAngle = Mathf.MoveTowards(
                _rightRuddervatorAngle,
                rightRuddervatorTarget,
                step);

            ApplyLocalXAxisRotation(
                _leftAileron,
                _leftAileronNeutralRotation,
                _leftAileronAngle);
            ApplyLocalXAxisRotation(
                _rightAileron,
                _rightAileronNeutralRotation,
                _rightAileronAngle);
            ApplyLocalXAxisRotation(
                _leftRuddervator,
                _leftRuddervatorNeutralRotation,
                _leftRuddervatorAngle);
            ApplyLocalXAxisRotation(
                _rightRuddervator,
                _rightRuddervatorNeutralRotation,
                _rightRuddervatorAngle);
        }

        private void OnDisable()
        {
            if (!_isInitialized)
            {
                return;
            }

            _leftAileron.localRotation = _leftAileronNeutralRotation;
            _rightAileron.localRotation = _rightAileronNeutralRotation;
            _leftRuddervator.localRotation = _leftRuddervatorNeutralRotation;
            _rightRuddervator.localRotation = _rightRuddervatorNeutralRotation;

            _leftAileronAngle = 0f;
            _rightAileronAngle = 0f;
            _leftRuddervatorAngle = 0f;
            _rightRuddervatorAngle = 0f;
        }

        private bool TryValidateReferences()
        {
            if (_inputReader != null &&
                _leftAileron != null &&
                _rightAileron != null &&
                _leftRuddervator != null &&
                _rightRuddervator != null)
            {
                return true;
            }

            Debug.LogError(
                $"{nameof(AircraftControlSurfaceAnimator)} requires an input reader and all four control surface references.",
                this);
            return false;
        }

        private static void ApplyLocalXAxisRotation(
            Transform surface,
            Quaternion neutralRotation,
            float angle)
        {
            surface.localRotation =
                neutralRotation * Quaternion.AngleAxis(angle, Vector3.right);
        }
    }
}
