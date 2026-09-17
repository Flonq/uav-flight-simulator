using MertKaan.UAVSimulator.Aircraft;
using NUnit.Framework;
using UnityEngine;

namespace MertKaan.UAVSimulator.Tests
{
    public sealed class AircraftPhysicsInertiaTests
    {
        private const float Tolerance = 0.0001f;

        [Test]
        public void IdentityInertiaRotation_UsesRigidbodyLocalAxes()
        {
            Vector3 localAngularAcceleration = new Vector3(1f, -2f, 0.5f);
            Vector3 inertiaTensor = new Vector3(2f, 3f, 5f);
            Quaternion bodyRotation = Quaternion.Euler(0f, 45f, 0f);

            Vector3 expectedLocalTorque = new Vector3(2f, -6f, 2.5f);
            Vector3 expectedWorldTorque = bodyRotation * expectedLocalTorque;

            Vector3 actualWorldTorque = AircraftPhysics.ConvertLocalAngularAccelerationToWorldTorque(
                localAngularAcceleration,
                inertiaTensor,
                Quaternion.identity,
                bodyRotation);

            Assert.That(actualWorldTorque.x, Is.EqualTo(expectedWorldTorque.x).Within(Tolerance));
            Assert.That(actualWorldTorque.y, Is.EqualTo(expectedWorldTorque.y).Within(Tolerance));
            Assert.That(actualWorldTorque.z, Is.EqualTo(expectedWorldTorque.z).Within(Tolerance));
        }

        [Test]
        public void NonIdentityInertiaRotation_UsesPrincipalAxesBeforeWorldConversion()
        {
            Vector3 localAngularAcceleration = new Vector3(1f, 2f, 3f);
            Vector3 inertiaTensor = new Vector3(2f, 3f, 5f);
            Quaternion inertiaTensorRotation = Quaternion.Euler(0f, 0f, 30f);
            Quaternion bodyRotation = Quaternion.Euler(20f, 35f, -10f);

            float cosine = Mathf.Cos(30f * Mathf.Deg2Rad);
            float sine = Mathf.Sin(30f * Mathf.Deg2Rad);
            Vector3 expectedPrincipalAcceleration = new Vector3(
                cosine * localAngularAcceleration.x + sine * localAngularAcceleration.y,
                -sine * localAngularAcceleration.x + cosine * localAngularAcceleration.y,
                localAngularAcceleration.z);
            Vector3 expectedPrincipalTorque = Vector3.Scale(
                inertiaTensor,
                expectedPrincipalAcceleration);
            Vector3 expectedLocalTorque = new Vector3(
                cosine * expectedPrincipalTorque.x - sine * expectedPrincipalTorque.y,
                sine * expectedPrincipalTorque.x + cosine * expectedPrincipalTorque.y,
                expectedPrincipalTorque.z);
            Vector3 expectedWorldTorque = bodyRotation * expectedLocalTorque;

            Vector3 actualWorldTorque = AircraftPhysics.ConvertLocalAngularAccelerationToWorldTorque(
                localAngularAcceleration,
                inertiaTensor,
                inertiaTensorRotation,
                bodyRotation);

            Assert.That(actualWorldTorque.x, Is.EqualTo(expectedWorldTorque.x).Within(Tolerance));
            Assert.That(actualWorldTorque.y, Is.EqualTo(expectedWorldTorque.y).Within(Tolerance));
            Assert.That(actualWorldTorque.z, Is.EqualTo(expectedWorldTorque.z).Within(Tolerance));
        }
    }
}
