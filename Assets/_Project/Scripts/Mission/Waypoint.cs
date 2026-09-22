using UnityEngine;

namespace MertKaan.UAVSimulator.Missions
{
    public enum WaypointType
    {
        Route,
        ReturnToBase
    }

    /// <summary>
    /// A serializable point in an ordered mission route. This component owns
    /// descriptive data and its optional presentation marker only; it never
    /// applies forces or participates in flight physics.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Waypoint : MonoBehaviour
    {
        [Header("Waypoint")]
        [SerializeField]
        private string _displayName = "Waypoint";

        [SerializeField, TextArea(1, 2)]
        private string _description = "Route waypoint";

        [SerializeField, Min(0.1f)]
        private float _detectionRadiusMeters = 25f;

        [SerializeField]
        private WaypointType _waypointType = WaypointType.Route;

        [Header("Presentation")]
        [SerializeField]
        private GameObject _activeVisual;

        public string DisplayName => string.IsNullOrWhiteSpace(_displayName)
            ? name
            : _displayName;

        public string Description => _description ?? string.Empty;

        public float DetectionRadiusMeters => _detectionRadiusMeters;

        public WaypointType WaypointType => _waypointType;

        public Vector3 WorldPosition => transform.position;

        public bool HasValidDetectionRadius =>
            !float.IsNaN(_detectionRadiusMeters) &&
            !float.IsInfinity(_detectionRadiusMeters) &&
            _detectionRadiusMeters > 0f;

        public void SetActiveVisual(bool isActive)
        {
            if (_activeVisual != null)
            {
                _activeVisual.SetActive(isActive);
            }
        }

        public bool IsWithinDetectionRadius(Vector3 aircraftWorldPosition)
        {
            if (!HasValidDetectionRadius)
            {
                return false;
            }

            return Vector3.Distance(aircraftWorldPosition, WorldPosition) <=
                _detectionRadiusMeters;
        }

        private void OnValidate()
        {
            if (float.IsNaN(_detectionRadiusMeters) ||
                float.IsInfinity(_detectionRadiusMeters))
            {
                _detectionRadiusMeters = 25f;
            }

            _detectionRadiusMeters = Mathf.Max(0.1f, _detectionRadiusMeters);
        }
    }
}
