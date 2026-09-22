using UnityEngine;

namespace MertKaan.UAVSimulator.UI.Production
{
    public static class ProductionMinimapMath
    {
        public static Vector2 WorldToNorthUpOffset(
            Vector3 aircraftWorldPosition,
            Vector3 targetWorldPosition,
            float visibleRadiusMeters,
            Vector2 mapSize)
        {
            if (!IsFinite(visibleRadiusMeters) ||
                visibleRadiusMeters <= Mathf.Epsilon ||
                !IsFinite(mapSize.x) ||
                !IsFinite(mapSize.y) ||
                mapSize.x <= 0f ||
                mapSize.y <= 0f)
            {
                return Vector2.zero;
            }

            Vector3 delta = targetWorldPosition - aircraftWorldPosition;
            float scale = Mathf.Min(mapSize.x, mapSize.y) / (2f * visibleRadiusMeters);
            Vector2 offset = new Vector2(delta.x * scale, delta.z * scale);
            return IsFinite(offset) ? offset : Vector2.zero;
        }

        public static Vector2 ClampToPanelEdge(Vector2 offset, Vector2 halfSize)
        {
            if (!IsFinite(offset) ||
                !IsFinite(halfSize) ||
                halfSize.x <= 0f ||
                halfSize.y <= 0f)
            {
                return Vector2.zero;
            }

            if (offset.sqrMagnitude <= Mathf.Epsilon)
            {
                return Vector2.zero;
            }

            float scale = 1f;
            if (Mathf.Abs(offset.x) > halfSize.x)
            {
                scale = Mathf.Min(scale, halfSize.x / Mathf.Abs(offset.x));
            }

            if (Mathf.Abs(offset.y) > halfSize.y)
            {
                scale = Mathf.Min(scale, halfSize.y / Mathf.Abs(offset.y));
            }

            return offset * scale;
        }

        public static float HeadingToArrowZRotation(float headingDegrees)
        {
            if (!IsFinite(headingDegrees))
            {
                return 0f;
            }

            return -Mathf.Repeat(headingDegrees, 360f);
        }

        public static float WorldDirectionToNorthUpZRotation(Vector3 worldDirection)
        {
            Vector3 horizontalDirection = Vector3.ProjectOnPlane(worldDirection, Vector3.up);
            if (horizontalDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                return 0f;
            }

            horizontalDirection.Normalize();
            float headingDegrees = Mathf.Atan2(horizontalDirection.x, horizontalDirection.z) * Mathf.Rad2Deg;
            return HeadingToArrowZRotation(headingDegrees);
        }

        public static float HorizontalDistance(Vector3 firstWorldPosition, Vector3 secondWorldPosition)
        {
            Vector3 delta = secondWorldPosition - firstWorldPosition;
            delta.y = 0f;
            return delta.magnitude;
        }

        private static bool IsFinite(Vector2 value)
        {
            return IsFinite(value.x) && IsFinite(value.y);
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
