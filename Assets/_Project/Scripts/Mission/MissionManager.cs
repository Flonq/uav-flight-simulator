using System;
using System.Collections.Generic;
using UnityEngine;

namespace MertKaan.UAVSimulator.Missions
{
    public enum MissionState
    {
        NotStarted,
        Active,
        RouteCompleted
    }

    /// <summary>
    /// Allocation-free value snapshot of the mission state at one physics step.
    /// Reference members point to the serialized waypoint data and are not
    /// created while the snapshot is published.
    /// </summary>
    public readonly struct MissionSnapshot
    {
        public readonly MissionState State;
        public readonly int ActiveWaypointIndex;
        public readonly int CompletedWaypointCount;
        public readonly int TotalWaypointCount;
        public readonly float ActiveWaypointDistanceMeters;
        public readonly Waypoint ActiveWaypoint;
        public readonly string ActiveWaypointName;
        public readonly string ActiveWaypointDescription;
        public readonly WaypointType ActiveWaypointType;

        public bool IsMissionStarted => State != MissionState.NotStarted;

        public bool RouteCompleted => State == MissionState.RouteCompleted;

        public MissionSnapshot(
            MissionState state,
            int activeWaypointIndex,
            int completedWaypointCount,
            int totalWaypointCount,
            float activeWaypointDistanceMeters,
            Waypoint activeWaypoint)
        {
            State = state;
            ActiveWaypointIndex = activeWaypointIndex;
            CompletedWaypointCount = completedWaypointCount;
            TotalWaypointCount = totalWaypointCount;
            ActiveWaypointDistanceMeters = activeWaypointDistanceMeters;
            ActiveWaypoint = activeWaypoint;
            ActiveWaypointName = activeWaypoint == null ? string.Empty : activeWaypoint.DisplayName;
            ActiveWaypointDescription = activeWaypoint == null ? string.Empty : activeWaypoint.Description;
            ActiveWaypointType = activeWaypoint == null
                ? WaypointType.Route
                : activeWaypoint.WaypointType;
        }
    }

    /// <summary>
    /// Owns an explicitly serialized, ordered waypoint route. Progression is
    /// evaluated once per FixedUpdate and advances at most one waypoint.
    /// </summary>
    [DefaultExecutionOrder(200)]
    [DisallowMultipleComponent]
    public sealed class MissionManager : MonoBehaviour
    {
        [Header("Mission Sources")]
        [SerializeField]
        private Transform _aircraftRoot;

        [SerializeField]
        private List<Waypoint> _waypoints = new List<Waypoint>();

        [SerializeField]
        private bool _startOnPlay;

        private MissionState _state = MissionState.NotStarted;
        private int _activeWaypointIndex = -1;
        private MissionSnapshot _currentSnapshot = new MissionSnapshot(
            MissionState.NotStarted,
            -1,
            0,
            0,
            0f,
            null);
        private bool _validationErrorReported;

        public MissionSnapshot CurrentSnapshot => _currentSnapshot;

        public MissionState State => _state;

        public int ActiveWaypointIndex => _currentSnapshot.ActiveWaypointIndex;

        public Waypoint ActiveWaypoint => _currentSnapshot.ActiveWaypoint;

        public float ActiveWaypointDistanceMeters =>
            _currentSnapshot.ActiveWaypointDistanceMeters;

        public bool IsMissionStarted => _state != MissionState.NotStarted;

        public bool RouteCompleted => _state == MissionState.RouteCompleted;

        public int TotalWaypointCount => _waypoints == null ? 0 : _waypoints.Count;

        public event Action<MissionSnapshot> SnapshotUpdated;

        private void Awake()
        {
            if (!ValidateConfiguration(true))
            {
                enabled = false;
                return;
            }

            _currentSnapshot = BuildSnapshot();
            SetOnlyActiveVisual(-1);
        }

        private void Start()
        {
            if (_startOnPlay)
            {
                StartMission();
            }
        }

        private void FixedUpdate()
        {
            if (_state == MissionState.NotStarted ||
                _state == MissionState.RouteCompleted)
            {
                return;
            }

            if (!ValidateRuntimeReferences())
            {
                enabled = false;
                return;
            }

            Waypoint currentWaypoint = _waypoints[_activeWaypointIndex];
            if (currentWaypoint.IsWithinDetectionRadius(_aircraftRoot.position))
            {
                _activeWaypointIndex++;
                if (_activeWaypointIndex >= _waypoints.Count)
                {
                    _state = MissionState.RouteCompleted;
                    SetOnlyActiveVisual(-1);
                }
                else
                {
                    SetOnlyActiveVisual(_activeWaypointIndex);
                }
            }

            PublishSnapshot();
        }

        /// <summary>
        /// Starts or restarts the ordered route through the public path that a
        /// future menu button can call. Returns false when configuration is invalid.
        /// </summary>
        public bool StartMission()
        {
            if (!ValidateConfiguration(true))
            {
                return false;
            }

            _state = MissionState.Active;
            _activeWaypointIndex = 0;
            SetOnlyActiveVisual(_activeWaypointIndex);
            PublishSnapshot();
            return true;
        }

        private void PublishSnapshot()
        {
            _currentSnapshot = BuildSnapshot();
            SnapshotUpdated?.Invoke(_currentSnapshot);
        }

        private MissionSnapshot BuildSnapshot()
        {
            Waypoint activeWaypoint = null;
            float distanceMeters = 0f;
            if (_state == MissionState.Active &&
                _activeWaypointIndex >= 0 &&
                _activeWaypointIndex < TotalWaypointCount)
            {
                activeWaypoint = _waypoints[_activeWaypointIndex];
                distanceMeters = Vector3.Distance(
                    _aircraftRoot.position,
                    activeWaypoint.WorldPosition);
                if (float.IsNaN(distanceMeters) || float.IsInfinity(distanceMeters))
                {
                    distanceMeters = 0f;
                }
            }

            int completedWaypointCount = _state == MissionState.RouteCompleted
                ? TotalWaypointCount
                : Mathf.Max(0, _activeWaypointIndex);

            return new MissionSnapshot(
                _state,
                _state == MissionState.Active ? _activeWaypointIndex : -1,
                completedWaypointCount,
                TotalWaypointCount,
                distanceMeters,
                activeWaypoint);
        }

        private bool ValidateConfiguration(bool logError)
        {
            if (_aircraftRoot == null)
            {
                return ReportValidationError(
                    "requires a serialized aircraft root Transform.",
                    logError);
            }

            if (_waypoints == null || _waypoints.Count == 0)
            {
                return ReportValidationError(
                    "requires at least one serialized waypoint.",
                    logError);
            }

            for (int index = 0; index < _waypoints.Count; index++)
            {
                Waypoint waypoint = _waypoints[index];
                if (waypoint == null)
                {
                    return ReportValidationError(
                        $"contains a null waypoint at index {index}.",
                        logError);
                }

                if (!waypoint.HasValidDetectionRadius)
                {
                    return ReportValidationError(
                        $"contains waypoint {index} with an invalid detection radius.",
                        logError);
                }
            }

            _validationErrorReported = false;
            return true;
        }

        private bool ValidateRuntimeReferences()
        {
            if (_aircraftRoot == null)
            {
                return ReportValidationError(
                    "lost its serialized aircraft root Transform during runtime.",
                    true);
            }

            if (_activeWaypointIndex < 0 || _activeWaypointIndex >= TotalWaypointCount)
            {
                return ReportValidationError(
                    "has an invalid active waypoint index.",
                    true);
            }

            return _waypoints[_activeWaypointIndex] != null;
        }

        private bool ReportValidationError(string message, bool logError)
        {
            if (logError && !_validationErrorReported)
            {
                Debug.LogError($"{nameof(MissionManager)} {message}", this);
                _validationErrorReported = true;
            }

            return false;
        }

        private void SetOnlyActiveVisual(int activeIndex)
        {
            if (_waypoints == null)
            {
                return;
            }

            for (int index = 0; index < _waypoints.Count; index++)
            {
                _waypoints[index]?.SetActiveVisual(index == activeIndex);
            }
        }
    }
}
