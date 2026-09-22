using MertKaan.UAVSimulator.Aircraft;
using MertKaan.UAVSimulator.Missions;
using MertKaan.UAVSimulator.Telemetry;
using MertKaan.UAVSimulator.UI.Production;
using NUnit.Framework;
using UnityEngine;

namespace MertKaan.UAVSimulator.Tests
{
    public sealed class ProductionHudTests
    {
        private const float Tolerance = 0.0001f;

        [Test]
        public void Formatting_UsesReadableFlightSpeedAndEngineStates()
        {
            Assert.That(
                ProductionHudFormatting.GetFlightStateLabel(AircraftFlightState.Grounded),
                Is.EqualTo("Grounded"));
            Assert.That(
                ProductionHudFormatting.GetFlightStateLabel(AircraftFlightState.Flying),
                Is.EqualTo("Flying"));
            Assert.That(
                ProductionHudFormatting.GetFlightStateLabel(AircraftFlightState.LowSpeed),
                Is.EqualTo("Low Speed"));
            Assert.That(
                ProductionHudFormatting.GetSpeedStateLabel(AircraftSpeedState.Caution),
                Is.EqualTo("Caution"));
            Assert.That(
                ProductionHudFormatting.GetSpeedStateLabel(AircraftSpeedState.Overspeed),
                Is.EqualTo("Overspeed"));
            Assert.That(
                ProductionHudFormatting.GetEngineStateLabel(true),
                Is.EqualTo("Running"));
            Assert.That(
                ProductionHudFormatting.GetEngineStateLabel(false),
                Is.EqualTo("Stopped"));
            Assert.That(
                ProductionHudFormatting.GetMissionStateLabel(MissionState.NotStarted),
                Is.EqualTo("Not Started"));
            Assert.That(
                ProductionHudFormatting.GetMissionStateLabel(MissionState.Active),
                Is.EqualTo("Active"));
            Assert.That(
                ProductionHudFormatting.GetMissionStateLabel(MissionState.RouteCompleted),
                Is.EqualTo("Route Completed"));
        }

        [Test]
        public void AlertSelection_DistinguishesNormalCautionOverspeedAndLowSpeed()
        {
            AircraftTelemetrySnapshot normal = CreateSnapshot(
                AircraftFlightState.Flying,
                AircraftSpeedState.Normal);
            AircraftTelemetrySnapshot caution = CreateSnapshot(
                AircraftFlightState.Flying,
                AircraftSpeedState.Caution);
            AircraftTelemetrySnapshot overspeed = CreateSnapshot(
                AircraftFlightState.Flying,
                AircraftSpeedState.Overspeed);
            AircraftTelemetrySnapshot lowSpeed = CreateSnapshot(
                AircraftFlightState.LowSpeed,
                AircraftSpeedState.Normal);

            Assert.That(
                ProductionHudFormatting.GetAlertLevel(normal),
                Is.EqualTo(ProductionHudAlertLevel.Normal));
            Assert.That(
                ProductionHudFormatting.GetAlertLevel(caution),
                Is.EqualTo(ProductionHudAlertLevel.Caution));
            Assert.That(
                ProductionHudFormatting.GetAlertLevel(overspeed),
                Is.EqualTo(ProductionHudAlertLevel.Danger));
            Assert.That(
                ProductionHudFormatting.GetAlertLevel(lowSpeed),
                Is.EqualTo(ProductionHudAlertLevel.Caution));
            Assert.That(ProductionHudFormatting.GetAlertLabel(normal), Is.EqualTo("NORMAL"));
            Assert.That(ProductionHudFormatting.GetAlertLabel(caution), Is.EqualTo("CAUTION"));
            Assert.That(ProductionHudFormatting.GetAlertLabel(overspeed), Is.EqualTo("OVERSPEED"));
            Assert.That(ProductionHudFormatting.GetAlertLabel(lowSpeed), Is.EqualTo("LOW SPEED"));
        }

        [Test]
        public void NorthUpMapping_UsesEastRightAndNorthUp()
        {
            Vector3 aircraft = Vector3.zero;
            Vector2 mapSize = new Vector2(200f, 200f);

            Assert.That(
                ProductionMinimapMath.WorldToNorthUpOffset(aircraft, Vector3.forward * 100f, 100f, mapSize),
                Is.EqualTo(new Vector2(0f, 100f)));
            Assert.That(
                ProductionMinimapMath.WorldToNorthUpOffset(aircraft, Vector3.right * 100f, 100f, mapSize),
                Is.EqualTo(new Vector2(100f, 0f)));
            Assert.That(
                ProductionMinimapMath.WorldToNorthUpOffset(aircraft, Vector3.back * 100f, 100f, mapSize),
                Is.EqualTo(new Vector2(0f, -100f)));
            Assert.That(
                ProductionMinimapMath.WorldToNorthUpOffset(aircraft, Vector3.left * 100f, 100f, mapSize),
                Is.EqualTo(new Vector2(-100f, 0f)));
        }

        [Test]
        public void HeadingArrowRotation_UsesNorthUpCardinalDirections()
        {
            Assert.That(ProductionMinimapMath.HeadingToArrowZRotation(0f), Is.EqualTo(0f).Within(Tolerance));
            Assert.That(ProductionMinimapMath.HeadingToArrowZRotation(90f), Is.EqualTo(-90f).Within(Tolerance));
            Assert.That(ProductionMinimapMath.HeadingToArrowZRotation(180f), Is.EqualTo(-180f).Within(Tolerance));
            Assert.That(ProductionMinimapMath.HeadingToArrowZRotation(270f), Is.EqualTo(-270f).Within(Tolerance));
        }

        [Test]
        public void RunwayIndicator_StaysInRangeAndClampsToCorrectPanelEdge()
        {
            Vector2 halfSize = new Vector2(100f, 100f);
            Vector2 inside = ProductionMinimapMath.ClampToPanelEdge(new Vector2(40f, -60f), halfSize);
            Vector2 outside = ProductionMinimapMath.ClampToPanelEdge(new Vector2(500f, 100f), halfSize);

            Assert.That(inside, Is.EqualTo(new Vector2(40f, -60f)));
            Assert.That(outside.x, Is.EqualTo(100f).Within(Tolerance));
            Assert.That(outside.y, Is.EqualTo(20f).Within(Tolerance));
        }

        [Test]
        public void MinimapMath_ZeroDistanceAndInvalidBoundsRemainFinite()
        {
            Vector2 zero = ProductionMinimapMath.WorldToNorthUpOffset(
                Vector3.one,
                Vector3.one,
                0f,
                Vector2.zero);
            Vector2 clamped = ProductionMinimapMath.ClampToPanelEdge(
                new Vector2(float.PositiveInfinity, 1f),
                new Vector2(100f, 100f));
            float distance = ProductionMinimapMath.HorizontalDistance(Vector3.one, Vector3.one);

            Assert.That(zero, Is.EqualTo(Vector2.zero));
            Assert.That(clamped, Is.EqualTo(Vector2.zero));
            Assert.That(distance, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(float.IsNaN(distance) || float.IsInfinity(distance), Is.False);
        }

        [Test]
        public void WaypointIndicator_UsesNorthUpDirectionAndClampsOutOfRange()
        {
            GameObject waypointObject = new GameObject("WaypointMathTest");
            Waypoint waypoint = waypointObject.AddComponent<Waypoint>();
            MissionSnapshot snapshot = new MissionSnapshot(
                MissionState.Active,
                0,
                0,
                1,
                50f,
                waypoint);

            Vector2 offset = ProductionMinimapMath.WorldToNorthUpOffset(
                Vector3.zero,
                Vector3.forward * 500f + Vector3.right * 100f,
                100f,
                new Vector2(200f, 200f));
            Vector2 clamped = ProductionMinimapMath.ClampToPanelEdge(offset, new Vector2(100f, 100f));

            Assert.That(ProductionFlightHud.ShouldShowWaypointIndicator(snapshot), Is.True);
            Assert.That(clamped.x, Is.EqualTo(20f).Within(Tolerance));
            Assert.That(clamped.y, Is.EqualTo(100f).Within(Tolerance));

            MissionSnapshot completed = new MissionSnapshot(
                MissionState.RouteCompleted,
                -1,
                1,
                1,
                0f,
                null);
            Assert.That(ProductionFlightHud.ShouldShowWaypointIndicator(completed), Is.False);

            Object.DestroyImmediate(waypointObject);
        }

        private static AircraftTelemetrySnapshot CreateSnapshot(
            AircraftFlightState flightState,
            AircraftSpeedState speedState)
        {
            return new AircraftTelemetrySnapshot(
                10f,
                12f,
                20f,
                -1f,
                90f,
                true,
                1f,
                2f,
                3f,
                40f,
                true,
                flightState,
                speedState);
        }
    }
}
