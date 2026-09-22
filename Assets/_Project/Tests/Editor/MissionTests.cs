using System.Collections.Generic;
using System.Reflection;
using MertKaan.UAVSimulator.Missions;
using NUnit.Framework;
using UnityEngine;

namespace MertKaan.UAVSimulator.Tests
{
    public sealed class MissionTests
    {
        private const float Tolerance = 0.0001f;

        private GameObject _aircraftObject;
        private GameObject _managerObject;
        private MissionManager _missionManager;
        private readonly List<GameObject> _waypointObjects = new List<GameObject>();

        [SetUp]
        public void SetUp()
        {
            _aircraftObject = new GameObject("MissionTestAircraft");
            _managerObject = new GameObject("MissionTestManager");
            _managerObject.SetActive(false);
            _missionManager = _managerObject.AddComponent<MissionManager>();

            Waypoint first = CreateWaypoint("First", Vector3.zero, 5f);
            Waypoint second = CreateWaypoint("Second", new Vector3(100f, 0f, 0f), 5f);
            SetPrivateField(_missionManager, "_aircraftRoot", _aircraftObject.transform);
            SetPrivateField(_missionManager, "_waypoints", new List<Waypoint> { first, second });
            _managerObject.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_managerObject);
            Object.DestroyImmediate(_aircraftObject);
            for (int index = 0; index < _waypointObjects.Count; index++)
            {
                Object.DestroyImmediate(_waypointObjects[index]);
            }

            _waypointObjects.Clear();
        }

        [Test]
        public void MissionDoesNotProgressBeforeStart()
        {
            InvokeFixedUpdate();

            Assert.That(_missionManager.State, Is.EqualTo(MissionState.NotStarted));
            Assert.That(_missionManager.ActiveWaypointIndex, Is.EqualTo(-1));
            Assert.That(_missionManager.CurrentSnapshot.ActiveWaypoint, Is.Null);
        }

        [Test]
        public void StartMissionActivatesFirstWaypoint()
        {
            int eventCount = 0;
            _missionManager.SnapshotUpdated += snapshot => eventCount++;

            Assert.That(_missionManager.StartMission(), Is.True);

            Assert.That(_missionManager.State, Is.EqualTo(MissionState.Active));
            Assert.That(_missionManager.ActiveWaypointIndex, Is.EqualTo(0));
            Assert.That(_missionManager.CurrentSnapshot.ActiveWaypointName, Is.EqualTo("First"));
            Assert.That(eventCount, Is.EqualTo(1));
        }

        [Test]
        public void AircraftOutsideDetectionRadiusDoesNotCompleteWaypoint()
        {
            _aircraftObject.transform.position = new Vector3(0f, 0f, 6f);
            _missionManager.StartMission();
            InvokeFixedUpdate();

            Assert.That(_missionManager.ActiveWaypointIndex, Is.EqualTo(0));
            Assert.That(_missionManager.CurrentSnapshot.ActiveWaypointDistanceMeters, Is.EqualTo(6f).Within(Tolerance));
        }

        [Test]
        public void DetectionRadiusBoundaryIsInclusive()
        {
            _aircraftObject.transform.position = new Vector3(5f, 0f, 0f);
            _missionManager.StartMission();
            InvokeFixedUpdate();

            Assert.That(_missionManager.ActiveWaypointIndex, Is.EqualTo(1));
            Assert.That(_missionManager.CurrentSnapshot.CompletedWaypointCount, Is.EqualTo(1));
        }

        [Test]
        public void AWaypointCompletesOnlyOnceWhileAircraftRemainsInsideIt()
        {
            _missionManager.StartMission();
            InvokeFixedUpdate();
            int activeIndexAfterFirstStep = _missionManager.ActiveWaypointIndex;
            InvokeFixedUpdate();

            Assert.That(activeIndexAfterFirstStep, Is.EqualTo(1));
            Assert.That(_missionManager.ActiveWaypointIndex, Is.EqualTo(1));
            Assert.That(_missionManager.State, Is.EqualTo(MissionState.Active));
        }

        [Test]
        public void WaypointsProgressInSerializedOrder()
        {
            _missionManager.StartMission();
            _aircraftObject.transform.position = Vector3.zero;
            InvokeFixedUpdate();
            Assert.That(_missionManager.ActiveWaypointIndex, Is.EqualTo(1));

            _aircraftObject.transform.position = new Vector3(100f, 0f, 0f);
            InvokeFixedUpdate();

            Assert.That(_missionManager.State, Is.EqualTo(MissionState.RouteCompleted));
            Assert.That(_missionManager.CurrentSnapshot.ActiveWaypoint, Is.Null);
        }

        [Test]
        public void OnePhysicsStepCannotSkipMoreThanOneWaypoint()
        {
            Waypoint third = CreateWaypoint("Third", Vector3.zero, 5f);
            List<Waypoint> route = new List<Waypoint>
            {
                GetWaypoint(0),
                GetWaypoint(1),
                third
            };
            SetPrivateField(_missionManager, "_waypoints", route);

            _missionManager.StartMission();
            InvokeFixedUpdate();

            Assert.That(_missionManager.State, Is.EqualTo(MissionState.Active));
            Assert.That(_missionManager.ActiveWaypointIndex, Is.EqualTo(1));
        }

        [Test]
        public void FinalWaypointSetsRouteCompletedWithoutMissionSuccess()
        {
            _missionManager.StartMission();
            _aircraftObject.transform.position = Vector3.zero;
            InvokeFixedUpdate();
            _aircraftObject.transform.position = new Vector3(100f, 0f, 0f);
            InvokeFixedUpdate();

            Assert.That(_missionManager.RouteCompleted, Is.True);
            Assert.That(_missionManager.CurrentSnapshot.RouteCompleted, Is.True);
            Assert.That(_missionManager.CurrentSnapshot.ActiveWaypoint, Is.Null);
            Assert.That(_missionManager.CurrentSnapshot.ActiveWaypointIndex, Is.EqualTo(-1));
            Assert.That(_missionManager.ActiveWaypointIndex, Is.EqualTo(-1));
        }

        [Test]
        public void EmptyRouteIsRejectedByConfigurationValidation()
        {
            SetPrivateField(_missionManager, "_waypoints", new List<Waypoint>());
            MethodInfo validationMethod = typeof(MissionManager).GetMethod(
                "ValidateConfiguration",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(validationMethod, Is.Not.Null);

            bool isValid = (bool)validationMethod.Invoke(_missionManager, new object[] { false });
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void SnapshotAndEventRemainConsistent()
        {
            MissionSnapshot received = default;
            _missionManager.SnapshotUpdated += snapshot => received = snapshot;
            _aircraftObject.transform.position = new Vector3(0f, 0f, 2f);

            _missionManager.StartMission();
            InvokeFixedUpdate();

            MissionSnapshot current = _missionManager.CurrentSnapshot;
            Assert.That(received.State, Is.EqualTo(current.State));
            Assert.That(received.ActiveWaypointIndex, Is.EqualTo(current.ActiveWaypointIndex));
            Assert.That(received.TotalWaypointCount, Is.EqualTo(current.TotalWaypointCount));
            Assert.That(received.ActiveWaypointDistanceMeters, Is.EqualTo(current.ActiveWaypointDistanceMeters).Within(Tolerance));
            Assert.That(received.ActiveWaypoint, Is.SameAs(current.ActiveWaypoint));
        }

        [Test]
        public void SnapshotEventIsRaisedOncePerPhysicsStep()
        {
            int eventCount = 0;
            _missionManager.SnapshotUpdated += snapshot => eventCount++;
            _missionManager.StartMission();
            InvokeFixedUpdate();
            InvokeFixedUpdate();

            Assert.That(eventCount, Is.EqualTo(3));
        }

        [Test]
        public void ActiveWaypointDistanceIsFiniteAndThreeDimensional()
        {
            _aircraftObject.transform.position = new Vector3(3f, 4f, 12f);
            _missionManager.StartMission();

            Assert.That(_missionManager.ActiveWaypointDistanceMeters, Is.EqualTo(13f).Within(Tolerance));
            Assert.That(float.IsNaN(_missionManager.ActiveWaypointDistanceMeters), Is.False);
            Assert.That(float.IsInfinity(_missionManager.ActiveWaypointDistanceMeters), Is.False);
        }

        [Test]
        public void ActiveVisualFollowsActiveWaypointAndHidesOnCompletion()
        {
            Waypoint first = GetWaypoint(0);
            GameObject marker = new GameObject("FirstMarker");
            marker.transform.SetParent(first.transform);
            SetPrivateField(first, "_activeVisual", marker);

            _missionManager.StartMission();
            Assert.That(marker.activeSelf, Is.True);

            _aircraftObject.transform.position = new Vector3(5f, 0f, 0f);
            InvokeFixedUpdate();
            Assert.That(marker.activeSelf, Is.False);
        }

        private Waypoint CreateWaypoint(string displayName, Vector3 position, float radius)
        {
            GameObject waypointObject = new GameObject(displayName);
            waypointObject.transform.position = position;
            Waypoint waypoint = waypointObject.AddComponent<Waypoint>();
            SetPrivateField(waypoint, "_displayName", displayName);
            SetPrivateField(waypoint, "_detectionRadiusMeters", radius);
            _waypointObjects.Add(waypointObject);
            return waypoint;
        }

        private Waypoint GetWaypoint(int index)
        {
            return _waypointObjects[index].GetComponent<Waypoint>();
        }

        private void InvokeFixedUpdate()
        {
            MethodInfo method = typeof(MissionManager).GetMethod(
                "FixedUpdate",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(_missionManager, null);
        }

        private static void SetPrivateField<T>(object target, string fieldName, T value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Missing field: {fieldName}");
            field.SetValue(target, value);
        }
    }
}
