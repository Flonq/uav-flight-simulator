using MertKaan.UAVSimulator.CameraSystem;
using MertKaan.UAVSimulator.Missions;
using MertKaan.UAVSimulator.Targeting;
using TMPro;
using UnityEngine;

namespace MertKaan.UAVSimulator.UI.Production
{
    [DisallowMultipleComponent]
    public sealed class EOOverlay : MonoBehaviour
    {
        [Header("Sources")]
        [SerializeField] private CameraModeController _cameraModeController;
        [SerializeField] private EOCameraRig _eoCameraRig;
        [SerializeField] private EOTargetingController _targetingController;
        [SerializeField] private MissionTargetObservation _missionObservation;

        [Header("UI")]
        [SerializeField] private GameObject _eoContent;
        [SerializeField] private TMP_Text _fovValue;
        [SerializeField] private TMP_Text _targetValue;
        [SerializeField] private TMP_Text _observationValue;
        [SerializeField] private TMP_Text _reticle;

        private bool _subscribed;

        private void OnEnable()
        {
            if (!HasValidReferences())
            {
                Debug.LogError(
                    $"{nameof(EOOverlay)} requires camera, EO rig, targeting and serialized UI references.",
                    this);
                enabled = false;
                return;
            }

            _cameraModeController.SnapshotUpdated += HandleCameraMode;
            _eoCameraRig.SnapshotUpdated += HandleCameraSnapshot;
            _targetingController.SnapshotUpdated += HandleTargetingSnapshot;
            _missionObservation.SnapshotUpdated += HandleMissionObservation;
            _subscribed = true;

            HandleCameraMode(_cameraModeController.CurrentSnapshot);
            HandleCameraSnapshot(_eoCameraRig.CurrentSnapshot);
            HandleTargetingSnapshot(_targetingController.CurrentSnapshot);
            HandleMissionObservation(_missionObservation.CurrentSnapshot);
        }

        private void OnDisable()
        {
            if (_subscribed)
            {
                _cameraModeController.SnapshotUpdated -= HandleCameraMode;
                _eoCameraRig.SnapshotUpdated -= HandleCameraSnapshot;
                _targetingController.SnapshotUpdated -= HandleTargetingSnapshot;
                _missionObservation.SnapshotUpdated -= HandleMissionObservation;
            }

            _subscribed = false;
        }

        private void HandleCameraMode(CameraModeSnapshot snapshot)
        {
            _eoContent.SetActive(snapshot.IsEOActive);
            _reticle.gameObject.SetActive(snapshot.IsEOActive);
        }

        private void HandleCameraSnapshot(EOCameraSnapshot snapshot)
        {
            _fovValue.SetText("FOV {0:0}°", snapshot.FieldOfViewDegrees);
        }

        private void HandleTargetingSnapshot(TargetingSnapshot snapshot)
        {
            _targetValue.SetText(ProductionHudFormatting.GetTargetingStateLabel(snapshot.State));
        }

        private void HandleMissionObservation(MissionObservationSnapshot snapshot)
        {
            if (snapshot.State == MissionObservationState.Observing)
            {
                _observationValue.SetText(
                    "Observe {0:0.0}/{1:0.0} s",
                    snapshot.ProgressSeconds,
                    snapshot.RequiredSeconds);
            }
            else if (snapshot.State == MissionObservationState.Observed)
            {
                _observationValue.SetText(
                    "Observed {0:0.0}/{1:0.0} s",
                    snapshot.ProgressSeconds,
                    snapshot.RequiredSeconds);
            }
            else
            {
                _observationValue.SetText("Mission target required");
            }
        }

        private bool HasValidReferences()
        {
            return _cameraModeController != null &&
                _eoCameraRig != null &&
                _targetingController != null &&
                _missionObservation != null &&
                _eoContent != null &&
                _fovValue != null &&
                _targetValue != null &&
                _observationValue != null &&
                _reticle != null;
        }
    }
}
