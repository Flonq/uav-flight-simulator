using UnityEngine;

namespace MertKaan.UAVSimulator.Targeting
{
    [DisallowMultipleComponent]
    public sealed class EOTarget : MonoBehaviour
    {
        [SerializeField] private string _displayName = "EO Target";
        [TextArea(1, 2)]
        [SerializeField] private string _description = "Mission observation target";
        [SerializeField] private Transform _aimPoint;
        [SerializeField, Min(0f)] private float _observationZoneRadiusMeters = 30f;

        public string DisplayName => _displayName;
        public string Description => _description;
        public Transform AimPoint => _aimPoint != null ? _aimPoint : transform;
        public Vector3 WorldPosition => AimPoint.position;
        public float ObservationZoneRadiusMeters => _observationZoneRadiusMeters;
        public bool IsTargetable => isActiveAndEnabled && gameObject.activeInHierarchy;

        public static EOTarget FromCollider(Collider collider)
        {
            return collider != null ? collider.GetComponentInParent<EOTarget>() : null;
        }
    }
}
