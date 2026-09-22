using MertKaan.UAVSimulator.Aircraft;
using MertKaan.UAVSimulator.Telemetry;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MertKaan.UAVSimulator.UI.Production
{
    [DisallowMultipleComponent]
    public sealed class ProductionFlightHud : MonoBehaviour
    {
        private static readonly Color NormalAlertColor = new Color(0.16f, 0.32f, 0.42f, 0.94f);
        private static readonly Color CautionAlertColor = new Color(0.62f, 0.42f, 0.08f, 0.96f);
        private static readonly Color DangerAlertColor = new Color(0.62f, 0.10f, 0.08f, 0.96f);

        [Header("Sources")]
        [SerializeField]
        private AircraftTelemetry _telemetry;

        [SerializeField]
        private BoxCollider _runwayCollider;

        [Header("Telemetry Values")]
        [SerializeField]
        private TMP_Text _airspeedValue;

        [SerializeField]
        private TMP_Text _altitudeValue;

        [SerializeField]
        private TMP_Text _verticalSpeedValue;

        [SerializeField]
        private TMP_Text _headingValue;

        [SerializeField]
        private TMP_Text _throttleValue;

        [SerializeField]
        private TMP_Text _engineValue;

        [Header("State and Warning")]
        [SerializeField]
        private TMP_Text _flightStateValue;

        [SerializeField]
        private TMP_Text _speedStateValue;

        [SerializeField]
        private TMP_Text _alertValue;

        [SerializeField]
        private Image _alertBackground;

        [Header("Minimap")]
        [SerializeField]
        private RectTransform _minimapFrame;

        [SerializeField]
        private RectTransform _aircraftArrow;

        [SerializeField]
        private RectTransform _runwayIndicator;

        [SerializeField]
        private TMP_Text _runwayDistanceValue;

        [SerializeField, Min(1f)]
        private float _minimapVisibleRadiusMeters = 250f;

        private bool _subscribed;

        private void OnEnable()
        {
            if (!HasValidReferences())
            {
                Debug.LogError(
                    $"{nameof(ProductionFlightHud)} requires AircraftTelemetry, the serialized runway BoxCollider, " +
                    "and all serialized TMP/Image UI references.",
                    this);
                enabled = false;
                return;
            }

            _telemetry.SnapshotUpdated += HandleSnapshotUpdated;
            _subscribed = true;
            HandleSnapshotUpdated(_telemetry.CurrentSnapshot);
        }

        private void OnDisable()
        {
            if (_subscribed && _telemetry != null)
            {
                _telemetry.SnapshotUpdated -= HandleSnapshotUpdated;
            }

            _subscribed = false;
        }

        private bool HasValidReferences()
        {
            return _telemetry != null &&
                _runwayCollider != null &&
                _airspeedValue != null &&
                _altitudeValue != null &&
                _verticalSpeedValue != null &&
                _headingValue != null &&
                _throttleValue != null &&
                _engineValue != null &&
                _flightStateValue != null &&
                _speedStateValue != null &&
                _alertValue != null &&
                _alertBackground != null &&
                _minimapFrame != null &&
                _aircraftArrow != null &&
                _runwayIndicator != null &&
                _runwayDistanceValue != null &&
                _minimapVisibleRadiusMeters > 0f;
        }

        private void HandleSnapshotUpdated(AircraftTelemetrySnapshot snapshot)
        {
            _airspeedValue.SetText("{0:0.0} m/s", snapshot.AirspeedMps);
            _altitudeValue.SetText("{0:0.0} m", snapshot.AltitudeMeters);
            _verticalSpeedValue.SetText("{0:0.0} m/s", snapshot.VerticalSpeedMps);

            if (snapshot.HeadingValid)
            {
                _headingValue.SetText("{0:0.0}°", snapshot.HeadingDegrees);
            }
            else
            {
                _headingValue.SetText("---");
            }

            _throttleValue.SetText("{0:0}%", snapshot.ThrottlePercent);
            _engineValue.SetText(ProductionHudFormatting.GetEngineStateLabel(snapshot.EngineRunning));
            _flightStateValue.SetText(ProductionHudFormatting.GetFlightStateLabel(snapshot.FlightState));
            _speedStateValue.SetText(ProductionHudFormatting.GetSpeedStateLabel(snapshot.SpeedState));

            ProductionHudAlertLevel alertLevel = ProductionHudFormatting.GetAlertLevel(snapshot);
            _alertValue.SetText(ProductionHudFormatting.GetAlertLabel(snapshot));
            _alertBackground.color = GetAlertColor(alertLevel);

            UpdateMinimap(snapshot);
        }

        private void UpdateMinimap(AircraftTelemetrySnapshot snapshot)
        {
            Vector3 aircraftPosition = _telemetry.transform.position;
            Vector3 runwayPosition = _runwayCollider.bounds.center;
            Vector2 mapSize = _minimapFrame.rect.size;
            Vector2 offset = ProductionMinimapMath.WorldToNorthUpOffset(
                aircraftPosition,
                runwayPosition,
                _minimapVisibleRadiusMeters,
                mapSize);
            Vector2 halfSize = mapSize * 0.5f;
            _runwayIndicator.anchoredPosition = ProductionMinimapMath.ClampToPanelEdge(offset, halfSize);

            _aircraftArrow.localRotation = Quaternion.Euler(
                0f,
                0f,
                snapshot.HeadingValid
                    ? ProductionMinimapMath.HeadingToArrowZRotation(snapshot.HeadingDegrees)
                    : 0f);

            BoxCollider runwayBox = _runwayCollider;
            Vector3 localLongAxis = runwayBox.size.x >= runwayBox.size.z
                ? Vector3.right
                : Vector3.forward;
            Vector3 worldLongAxis = runwayBox.transform.TransformDirection(localLongAxis);
            _runwayIndicator.localRotation = Quaternion.Euler(
                0f,
                0f,
                ProductionMinimapMath.WorldDirectionToNorthUpZRotation(worldLongAxis));

            float runwayDistance = ProductionMinimapMath.HorizontalDistance(aircraftPosition, runwayPosition);
            _runwayDistanceValue.SetText("{0:0} m", runwayDistance);
        }

        private static Color GetAlertColor(ProductionHudAlertLevel alertLevel)
        {
            switch (alertLevel)
            {
                case ProductionHudAlertLevel.Caution:
                    return CautionAlertColor;
                case ProductionHudAlertLevel.Danger:
                    return DangerAlertColor;
                default:
                    return NormalAlertColor;
            }
        }
    }
}
