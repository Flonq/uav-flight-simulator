using UnityEngine;

namespace MertKaan.UAVSimulator.Aircraft
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AircraftEngine))]
    public sealed class AircraftPropellerAnimator : MonoBehaviour
    {
        private const float DegreesPerMinuteToDegreesPerSecond = 6f;

        [Header("References")]
        [SerializeField]
        private AircraftEngine _engine;

        [SerializeField]
        private Transform _rotorPivot;

        [Header("Rotation")]
        [SerializeField]
        private bool _reverseRotation;

        [SerializeField, Range(0.01f, 1f)]
        private float _visualRpmScale = 0.1f;

        private Quaternion _neutralLocalRotation;
        private float _rotationAngle;
        private bool _isInitialized;

        private void Reset()
        {
            _engine = GetComponent<AircraftEngine>();
        }

        private void OnValidate()
        {
            _visualRpmScale = Mathf.Clamp(_visualRpmScale, 0.01f, 1f);
        }

        private void Awake()
        {
            if (!TryValidateReferences())
            {
                enabled = false;
                return;
            }

            _neutralLocalRotation = _rotorPivot.localRotation;
            _rotationAngle = 0f;
            _isInitialized = true;
        }

        private void LateUpdate()
        {
            float direction = _reverseRotation ? -1f : 1f;
            float degreesPerSecond =
                _engine.Rpm * DegreesPerMinuteToDegreesPerSecond * _visualRpmScale;
            _rotationAngle = Mathf.Repeat(
                _rotationAngle + direction * degreesPerSecond * Time.deltaTime,
                360f);

            _rotorPivot.localRotation =
                _neutralLocalRotation * Quaternion.AngleAxis(_rotationAngle, Vector3.up);
        }

        private void OnDisable()
        {
            if (!_isInitialized)
            {
                return;
            }

            _rotorPivot.localRotation = _neutralLocalRotation;
            _rotationAngle = 0f;
        }

        private bool TryValidateReferences()
        {
            if (_engine != null &&
                _engine.gameObject == gameObject &&
                _rotorPivot != null &&
                _rotorPivot.IsChildOf(transform))
            {
                return true;
            }

            Debug.LogError(
                $"{nameof(AircraftPropellerAnimator)} requires the root engine and a child rotor pivot.",
                this);
            return false;
        }
    }
}
