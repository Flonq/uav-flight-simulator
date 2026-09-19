using System.Collections.Generic;
using MertKaan.UAVSimulator.Aircraft;
using NUnit.Framework;
using UnityEngine;

namespace MertKaan.UAVSimulator.Tests
{
    public sealed class AircraftGroundControllerSurfaceTests
    {
        private readonly List<GameObject> _createdObjects = new List<GameObject>();

        [TearDown]
        public void TearDown()
        {
            for (int i = _createdObjects.Count - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(_createdObjects[i]);
            }

            _createdObjects.Clear();
        }

        [Test]
        public void SameNamedColliders_UseExactRunwayReference()
        {
            BoxCollider runwayCollider = CreateBoxCollider("Tile_4");
            BoxCollider otherCollider = CreateBoxCollider("Tile_4");

            Assert.That(
                AircraftGroundController.ClassifySurface(
                    runwayCollider,
                    new Collider[] { runwayCollider }),
                Is.EqualTo(GroundSurfaceType.Runway));
            Assert.That(
                AircraftGroundController.ClassifySurface(
                    otherCollider,
                    new Collider[] { runwayCollider }),
                Is.EqualTo(GroundSurfaceType.Other));
        }

        [Test]
        public void TerrainCollider_IsClassifiedAsTerrain()
        {
            GameObject terrainObject = CreateObject("Terrain");
            TerrainCollider terrainCollider = terrainObject.AddComponent<TerrainCollider>();

            Assert.That(
                AircraftGroundController.ClassifySurface(
                    terrainCollider,
                    new Collider[0]),
                Is.EqualTo(GroundSurfaceType.Terrain));
        }

        [Test]
        public void MissingContact_IsClassifiedAsNone()
        {
            Assert.That(
                AircraftGroundController.ClassifySurface(null, null),
                Is.EqualTo(GroundSurfaceType.None));
        }

        [Test]
        public void MixedRunwayTerrainAndNone_SeparatesContactCounts()
        {
            AircraftGroundController.GroundSurfaceSummary summary =
                AircraftGroundController.SummarizeSurfaceContacts(
                    GroundSurfaceType.Runway,
                    GroundSurfaceType.Terrain,
                    GroundSurfaceType.None);

            Assert.That(summary.RunwayContactCount, Is.EqualTo(1));
            Assert.That(summary.OffRunwayContactCount, Is.EqualTo(1));
            Assert.That(summary.AnyWheelOnRunway, Is.True);
            Assert.That(summary.AllContactingWheelsOnRunway, Is.False);
        }

        [Test]
        public void AllTerrainContacts_HaveNoRunwayContact()
        {
            AircraftGroundController.GroundSurfaceSummary summary =
                AircraftGroundController.SummarizeSurfaceContacts(
                    GroundSurfaceType.Terrain,
                    GroundSurfaceType.Terrain,
                    GroundSurfaceType.Terrain);

            Assert.That(summary.RunwayContactCount, Is.EqualTo(0));
            Assert.That(summary.OffRunwayContactCount, Is.EqualTo(3));
            Assert.That(summary.AnyWheelOnRunway, Is.False);
            Assert.That(summary.AllContactingWheelsOnRunway, Is.False);
        }

        [Test]
        public void AllContactingWheelsOnRunway_ExcludesMissingWheel()
        {
            AircraftGroundController.GroundSurfaceSummary summary =
                AircraftGroundController.SummarizeSurfaceContacts(
                    GroundSurfaceType.Runway,
                    GroundSurfaceType.Runway,
                    GroundSurfaceType.None);

            Assert.That(summary.RunwayContactCount, Is.EqualTo(2));
            Assert.That(summary.OffRunwayContactCount, Is.EqualTo(0));
            Assert.That(summary.AnyWheelOnRunway, Is.True);
            Assert.That(summary.AllContactingWheelsOnRunway, Is.True);
        }

        private BoxCollider CreateBoxCollider(string name)
        {
            return CreateObject(name).AddComponent<BoxCollider>();
        }

        private GameObject CreateObject(string name)
        {
            GameObject gameObject = new GameObject(name);
            _createdObjects.Add(gameObject);
            return gameObject;
        }
    }
}
