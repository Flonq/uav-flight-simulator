using MertKaan.UAVSimulator.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MertKaan.UAVSimulator.Aircraft
{
    public enum GroundSurfaceType
    {
        None,
        Runway,
        Terrain,
        Other
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(AircraftInputReader))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class AircraftGroundController : MonoBehaviour
    {
        private const int RaycastBufferSize = 16;

        [Header("References")]
        [SerializeField]
        private AircraftInputReader _inputReader;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private SphereCollider _noseWheel;

        [SerializeField]
        private SphereCollider _rightMainWheel;

        [SerializeField]
        private SphereCollider _leftMainWheel;

        [Tooltip("Exact scene collider references that represent the usable runway surface.")]
        [SerializeField]
        private Collider[] _runwayColliders = new Collider[0];

        [Header("Ground Detection")]
        [SerializeField]
        private LayerMask _groundLayers = Physics.DefaultRaycastLayers;

        [SerializeField, Min(0.01f)]
        private float _groundCheckDistance = 0.2f;

        [SerializeField, Range(0f, 1f)]
        private float _minimumGroundNormalDot = 0.5f;

        [Header("Ground Handling")]
        [SerializeField, Min(0f)]
        private float _lateralGrip = 8f;

        [SerializeField, Min(0f)]
        private float _rollingResistance = 0.8f;

        [SerializeField, Min(0f)]
        private float _brakeDeceleration = 12f;

        [Header("Steering")]
        [SerializeField, Min(0f)]
        private float _maximumSteeringAcceleration = 1.5f;

        [SerializeField, Min(0f)]
        private float _minimumSteeringSpeed = 1f;

        [SerializeField, Min(0.1f)]
        private float _fullSteeringSpeed = 6f;

        [SerializeField, Min(0.1f)]
        private float _steeringFadeSpeed = 20f;

        [Header("Runway Stability")]
        [SerializeField, Min(0f)]
        private float _groundAlignmentStrength = 30f;

        [SerializeField, Min(0f)]
        private float _groundAlignmentDamping = 10f;

        [SerializeField, Min(0f)]
        private float _groundYawDamping = 6f;

        [SerializeField, Min(0f)]
        private float _fullGroundAlignmentSpeed = 8f;

        [SerializeField, Min(0.1f)]
        private float _groundAlignmentReleaseSpeed = 15f;

        public bool IsGrounded { get; private set; }
        public bool NoseWheelGrounded { get; private set; }
        public bool RightMainWheelGrounded { get; private set; }
        public bool LeftMainWheelGrounded { get; private set; }
        public int GroundedWheelCount { get; private set; }
        public GroundSurfaceType NoseWheelSurface { get; private set; }
        public GroundSurfaceType RightMainWheelSurface { get; private set; }
        public GroundSurfaceType LeftMainWheelSurface { get; private set; }
        public Collider NoseWheelContactCollider { get; private set; }
        public Collider RightMainWheelContactCollider { get; private set; }
        public Collider LeftMainWheelContactCollider { get; private set; }
        public Vector3 NoseWheelContactNormal { get; private set; }
        public Vector3 RightMainWheelContactNormal { get; private set; }
        public Vector3 LeftMainWheelContactNormal { get; private set; }
        public int RunwayContactCount { get; private set; }
        public int OffRunwayContactCount { get; private set; }
        public bool AnyWheelOnRunway { get; private set; }
        public bool AllContactingWheelsOnRunway { get; private set; }
        public bool IsBraking { get; private set; }
        public float ForwardGroundSpeed { get; private set; }
        public float LateralGroundSpeed { get; private set; }
        public Vector3 GroundNormal { get; private set; } = Vector3.up;
        public Vector3 GroundAcceleration { get; private set; }
        public Vector3 GroundAngularAcceleration { get; private set; }

        private readonly RaycastHit[] _raycastHits =
            new RaycastHit[RaycastBufferSize];

        private void Reset()
        {
            _inputReader = GetComponent<AircraftInputReader>();
            _rigidbody = GetComponent<Rigidbody>();
            _noseWheel = FindWheel("NoseWheelCollider");
            _rightMainWheel = FindWheel("RightMainWheelCollider");
            _leftMainWheel = FindWheel("LeftMainWheelCollider");
        }

        private void OnValidate()
        {
            _groundCheckDistance = Mathf.Max(0.01f, _groundCheckDistance);
            _minimumGroundNormalDot = Mathf.Clamp01(_minimumGroundNormalDot);
            _lateralGrip = Mathf.Max(0f, _lateralGrip);
            _rollingResistance = Mathf.Max(0f, _rollingResistance);
            _brakeDeceleration = Mathf.Max(0f, _brakeDeceleration);
            _maximumSteeringAcceleration = Mathf.Max(0f, _maximumSteeringAcceleration);
            _minimumSteeringSpeed = Mathf.Max(0f, _minimumSteeringSpeed);
            _fullSteeringSpeed = Mathf.Max(_minimumSteeringSpeed + 0.1f, _fullSteeringSpeed);
            _steeringFadeSpeed = Mathf.Max(_fullSteeringSpeed + 0.1f, _steeringFadeSpeed);
            _groundAlignmentStrength = Mathf.Max(0f, _groundAlignmentStrength);
            _groundAlignmentDamping = Mathf.Max(0f, _groundAlignmentDamping);
            _groundYawDamping = Mathf.Max(0f, _groundYawDamping);
            _fullGroundAlignmentSpeed = Mathf.Max(0f, _fullGroundAlignmentSpeed);
            _groundAlignmentReleaseSpeed = Mathf.Max(
                _fullGroundAlignmentSpeed + 0.1f,
                _groundAlignmentReleaseSpeed);
        }

        private void Awake()
        {
            if (!TryValidateReferences())
            {
                Debug.LogError(
                    $"{nameof(AircraftGroundController)} requires its root Input Reader, Rigidbody and three wheel SphereColliders.",
                    this);
                enabled = false;
            }
        }

        private void FixedUpdate()
        {
            UpdateGroundState();

            if (!IsGrounded || _rigidbody.isKinematic)
            {
                ResetForceDiagnostics();
                return;
            }

            Vector3 groundForward = Vector3.ProjectOnPlane(
                _rigidbody.rotation * Vector3.back,
                GroundNormal);
            if (groundForward.sqrMagnitude <= Mathf.Epsilon)
            {
                ResetForceDiagnostics();
                return;
            }

            groundForward.Normalize();
            Vector3 groundRight = Vector3.Cross(groundForward, GroundNormal).normalized;
            Vector3 groundVelocity = Vector3.ProjectOnPlane(
                _rigidbody.linearVelocity,
                GroundNormal);

            ForwardGroundSpeed = Vector3.Dot(groundVelocity, groundForward);
            LateralGroundSpeed = Vector3.Dot(groundVelocity, groundRight);
            IsBraking = _inputReader.isActiveAndEnabled &&
                _inputReader.BrakePressed &&
                (RightMainWheelGrounded || LeftMainWheelGrounded);

            GroundAcceleration = CalculateGroundAcceleration(
                groundForward,
                groundRight,
                Time.fixedDeltaTime);
            ApplyGroundYawDamping(Time.fixedDeltaTime);
            GroundAngularAcceleration = CalculateGroundAngularAcceleration();

            _rigidbody.AddForce(GroundAcceleration, ForceMode.Acceleration);
            _rigidbody.AddTorque(
                GroundAngularAcceleration,
                ForceMode.Acceleration);
        }

        private void UpdateGroundState()
        {
            PhysicsScene physicsScene = gameObject.scene.GetPhysicsScene();
            NoseWheelGrounded = TryGetGroundHit(
                physicsScene,
                _noseWheel,
                out RaycastHit noseHit);
            RightMainWheelGrounded = TryGetGroundHit(
                physicsScene,
                _rightMainWheel,
                out RaycastHit rightHit);
            LeftMainWheelGrounded = TryGetGroundHit(
                physicsScene,
                _leftMainWheel,
                out RaycastHit leftHit);

            Collider noseContactCollider;
            Vector3 noseContactNormal;
            NoseWheelSurface = GetContactSurface(
                NoseWheelGrounded,
                noseHit,
                out noseContactCollider,
                out noseContactNormal);
            NoseWheelContactCollider = noseContactCollider;
            NoseWheelContactNormal = noseContactNormal;

            Collider rightContactCollider;
            Vector3 rightContactNormal;
            RightMainWheelSurface = GetContactSurface(
                RightMainWheelGrounded,
                rightHit,
                out rightContactCollider,
                out rightContactNormal);
            RightMainWheelContactCollider = rightContactCollider;
            RightMainWheelContactNormal = rightContactNormal;

            Collider leftContactCollider;
            Vector3 leftContactNormal;
            LeftMainWheelSurface = GetContactSurface(
                LeftMainWheelGrounded,
                leftHit,
                out leftContactCollider,
                out leftContactNormal);
            LeftMainWheelContactCollider = leftContactCollider;
            LeftMainWheelContactNormal = leftContactNormal;

            GroundedWheelCount = 0;
            Vector3 normalSum = Vector3.zero;
            AddGroundHit(NoseWheelGrounded, noseHit, ref normalSum);
            AddGroundHit(RightMainWheelGrounded, rightHit, ref normalSum);
            AddGroundHit(LeftMainWheelGrounded, leftHit, ref normalSum);

            GroundSurfaceSummary surfaceSummary = SummarizeSurfaceContacts(
                NoseWheelSurface,
                RightMainWheelSurface,
                LeftMainWheelSurface);
            RunwayContactCount = surfaceSummary.RunwayContactCount;
            OffRunwayContactCount = surfaceSummary.OffRunwayContactCount;
            AnyWheelOnRunway = surfaceSummary.AnyWheelOnRunway;
            AllContactingWheelsOnRunway = surfaceSummary.AllContactingWheelsOnRunway;

            IsGrounded = GroundedWheelCount > 0;
            GroundNormal = IsGrounded ? normalSum.normalized : Vector3.up;
        }

        private void AddGroundHit(
            bool grounded,
            RaycastHit hit,
            ref Vector3 normalSum)
        {
            if (!grounded)
            {
                return;
            }

            GroundedWheelCount++;
            normalSum += hit.normal;
        }

        private GroundSurfaceType GetContactSurface(
            bool grounded,
            RaycastHit hit,
            out Collider contactCollider,
            out Vector3 contactNormal)
        {
            if (!grounded)
            {
                contactCollider = null;
                contactNormal = Vector3.zero;
                return GroundSurfaceType.None;
            }

            contactCollider = hit.collider;
            contactNormal = hit.normal;
            return ClassifySurface(contactCollider, _runwayColliders);
        }

        public static GroundSurfaceType ClassifySurface(
            Collider contactCollider,
            Collider[] runwayColliders)
        {
            if (contactCollider == null)
            {
                return GroundSurfaceType.None;
            }

            if (runwayColliders != null)
            {
                for (int i = 0; i < runwayColliders.Length; i++)
                {
                    if (runwayColliders[i] == contactCollider)
                    {
                        return GroundSurfaceType.Runway;
                    }
                }
            }

            return contactCollider is TerrainCollider
                ? GroundSurfaceType.Terrain
                : GroundSurfaceType.Other;
        }

        public readonly struct GroundSurfaceSummary
        {
            public GroundSurfaceSummary(
                int runwayContactCount,
                int offRunwayContactCount,
                bool anyWheelOnRunway,
                bool allContactingWheelsOnRunway)
            {
                RunwayContactCount = runwayContactCount;
                OffRunwayContactCount = offRunwayContactCount;
                AnyWheelOnRunway = anyWheelOnRunway;
                AllContactingWheelsOnRunway = allContactingWheelsOnRunway;
            }

            public int RunwayContactCount { get; }
            public int OffRunwayContactCount { get; }
            public bool AnyWheelOnRunway { get; }
            public bool AllContactingWheelsOnRunway { get; }
        }

        public static GroundSurfaceSummary SummarizeSurfaceContacts(
            GroundSurfaceType noseWheelSurface,
            GroundSurfaceType rightMainWheelSurface,
            GroundSurfaceType leftMainWheelSurface)
        {
            int runwayCount = 0;
            int offRunwayCount = 0;
            int contactCount = 0;
            AccumulateSurfaceContact(
                noseWheelSurface,
                ref runwayCount,
                ref offRunwayCount,
                ref contactCount);
            AccumulateSurfaceContact(
                rightMainWheelSurface,
                ref runwayCount,
                ref offRunwayCount,
                ref contactCount);
            AccumulateSurfaceContact(
                leftMainWheelSurface,
                ref runwayCount,
                ref offRunwayCount,
                ref contactCount);

            return new GroundSurfaceSummary(
                runwayCount,
                offRunwayCount,
                runwayCount > 0,
                contactCount > 0 && runwayCount == contactCount);
        }

        private static void AccumulateSurfaceContact(
            GroundSurfaceType surface,
            ref int runwayCount,
            ref int offRunwayCount,
            ref int contactCount)
        {
            if (surface == GroundSurfaceType.None)
            {
                return;
            }

            contactCount++;
            if (surface == GroundSurfaceType.Runway)
            {
                runwayCount++;
            }
            else
            {
                offRunwayCount++;
            }
        }

        private bool TryGetGroundHit(
            PhysicsScene physicsScene,
            SphereCollider wheel,
            out RaycastHit nearestHit)
        {
            Vector3 origin = wheel.transform.TransformPoint(wheel.center);
            float maxDistance = GetWorldRadius(wheel) + _groundCheckDistance;
            int hitCount = physicsScene.Raycast(
                origin,
                Vector3.down,
                _raycastHits,
                maxDistance,
                _groundLayers,
                QueryTriggerInteraction.Ignore);

            bool found = false;
            float nearestDistance = float.PositiveInfinity;
            nearestHit = default;
            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = _raycastHits[i];
                if (hit.collider == null ||
                    hit.collider.attachedRigidbody == _rigidbody ||
                    hit.collider.transform.IsChildOf(transform) ||
                    Vector3.Dot(hit.normal, Vector3.up) < _minimumGroundNormalDot ||
                    hit.distance >= nearestDistance)
                {
                    continue;
                }

                found = true;
                nearestDistance = hit.distance;
                nearestHit = hit;
            }

            return found;
        }

        private Vector3 CalculateGroundAcceleration(
            Vector3 groundForward,
            Vector3 groundRight,
            float deltaTime)
        {
            float contactFactor = GroundedWheelCount / 3f;
            float lateralDelta = -LateralGroundSpeed *
                Mathf.Clamp01(_lateralGrip * contactFactor * deltaTime);

            int groundedMainWheels =
                (RightMainWheelGrounded ? 1 : 0) +
                (LeftMainWheelGrounded ? 1 : 0);
            float deceleration = _rollingResistance * contactFactor;
            if (IsBraking && groundedMainWheels > 0)
            {
                deceleration += _brakeDeceleration;
            }

            float nextForwardSpeed = Mathf.MoveTowards(
                ForwardGroundSpeed,
                0f,
                deceleration * deltaTime);
            float forwardDelta = nextForwardSpeed - ForwardGroundSpeed;

            return groundForward * (forwardDelta / deltaTime) +
                groundRight * (lateralDelta / deltaTime);
        }

        private Vector3 CalculateGroundAngularAcceleration()
        {
            Vector3 angularAcceleration = Vector3.zero;
            if (NoseWheelGrounded && _inputReader.isActiveAndEnabled)
            {
                float speed = Mathf.Abs(ForwardGroundSpeed);
                float lowSpeedAuthority = Mathf.InverseLerp(
                    _minimumSteeringSpeed,
                    _fullSteeringSpeed,
                    speed);
                float highSpeedAuthority = 1f - Mathf.InverseLerp(
                    _fullSteeringSpeed,
                    _steeringFadeSpeed,
                    speed);
                float steeringAuthority = lowSpeedAuthority * highSpeedAuthority;
                angularAcceleration += -GroundNormal *
                    (_inputReader.Yaw * _maximumSteeringAcceleration * steeringAuthority);
            }

            float alignmentAuthority = 1f - Mathf.InverseLerp(
                _fullGroundAlignmentSpeed,
                _groundAlignmentReleaseSpeed,
                Mathf.Abs(ForwardGroundSpeed));
            if (alignmentAuthority > 0f)
            {
                Vector3 aircraftUp = _rigidbody.rotation * Vector3.up;
                Vector3 alignmentAxis = Vector3.Cross(aircraftUp, GroundNormal);
                Vector3 tiltAngularVelocity = Vector3.ProjectOnPlane(
                    _rigidbody.angularVelocity,
                    GroundNormal);
                angularAcceleration += (
                    alignmentAxis * _groundAlignmentStrength -
                    tiltAngularVelocity * _groundAlignmentDamping) *
                    alignmentAuthority;
            }

            return angularAcceleration;
        }

        private void ApplyGroundYawDamping(float deltaTime)
        {
            float groundYawSpeed = Vector3.Dot(
                _rigidbody.angularVelocity,
                GroundNormal);
            float damping = 1f - Mathf.Exp(-_groundYawDamping * deltaTime);
            _rigidbody.angularVelocity -=
                GroundNormal * (groundYawSpeed * damping);
        }

        private bool TryValidateReferences()
        {
            return _inputReader != null &&
                _inputReader.gameObject == gameObject &&
                _rigidbody != null &&
                _rigidbody.gameObject == gameObject &&
                IsValidWheel(_noseWheel) &&
                IsValidWheel(_rightMainWheel) &&
                IsValidWheel(_leftMainWheel);
        }

        private bool IsValidWheel(SphereCollider wheel)
        {
            return wheel != null &&
                wheel.transform.IsChildOf(transform) &&
                wheel.attachedRigidbody == _rigidbody &&
                !wheel.isTrigger;
        }

        private SphereCollider FindWheel(string wheelName)
        {
            SphereCollider[] wheels = GetComponentsInChildren<SphereCollider>(true);
            for (int i = 0; i < wheels.Length; i++)
            {
                if (wheels[i].name == wheelName)
                {
                    return wheels[i];
                }
            }

            return null;
        }

        private static float GetWorldRadius(SphereCollider wheel)
        {
            Vector3 scale = wheel.transform.lossyScale;
            float maximumScale = Mathf.Max(
                Mathf.Abs(scale.x),
                Mathf.Abs(scale.y),
                Mathf.Abs(scale.z));
            return wheel.radius * maximumScale;
        }

        private void ResetForceDiagnostics()
        {
            IsBraking = false;
            ForwardGroundSpeed = 0f;
            LateralGroundSpeed = 0f;
            GroundAcceleration = Vector3.zero;
            GroundAngularAcceleration = Vector3.zero;
        }

        private void OnDisable()
        {
            IsGrounded = false;
            NoseWheelGrounded = false;
            RightMainWheelGrounded = false;
            LeftMainWheelGrounded = false;
            GroundedWheelCount = 0;
            NoseWheelSurface = GroundSurfaceType.None;
            RightMainWheelSurface = GroundSurfaceType.None;
            LeftMainWheelSurface = GroundSurfaceType.None;
            NoseWheelContactCollider = null;
            RightMainWheelContactCollider = null;
            LeftMainWheelContactCollider = null;
            NoseWheelContactNormal = Vector3.zero;
            RightMainWheelContactNormal = Vector3.zero;
            LeftMainWheelContactNormal = Vector3.zero;
            RunwayContactCount = 0;
            OffRunwayContactCount = 0;
            AnyWheelOnRunway = false;
            AllContactingWheelsOnRunway = false;
            GroundNormal = Vector3.up;
            ResetForceDiagnostics();
        }

        private void OnDrawGizmosSelected()
        {
            DrawGroundProbe(_noseWheel);
            DrawGroundProbe(_rightMainWheel);
            DrawGroundProbe(_leftMainWheel);
        }

        private void DrawGroundProbe(SphereCollider wheel)
        {
            if (wheel == null)
            {
                return;
            }

            Vector3 origin = wheel.transform.TransformPoint(wheel.center);
            float length = GetWorldRadius(wheel) + _groundCheckDistance;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(origin, origin + Vector3.down * length);
        }
    }
}
