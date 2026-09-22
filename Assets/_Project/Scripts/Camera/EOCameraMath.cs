using UnityEngine;

namespace MertKaan.UAVSimulator.CameraSystem
{
    public static class EOCameraMath
    {
        public static float ClampYaw(float yawDegrees, float limitDegrees)
        {
            float limit = Mathf.Max(0f, limitDegrees);
            return Mathf.Clamp(yawDegrees, -limit, limit);
        }

        public static float ClampPitch(float pitchDegrees, float minimumDegrees, float maximumDegrees)
        {
            return Mathf.Clamp(pitchDegrees, Mathf.Min(minimumDegrees, maximumDegrees), Mathf.Max(minimumDegrees, maximumDegrees));
        }

        public static float ClampFieldOfView(float fieldOfView, float minimumDegrees, float maximumDegrees)
        {
            return Mathf.Clamp(fieldOfView, Mathf.Max(1f, minimumDegrees), Mathf.Max(minimumDegrees, maximumDegrees));
        }

        /// <summary>
        /// Converts a mouse pixel delta to local gimbal-angle deltas.
        /// Positive screen X moves the EO view right and positive screen Y moves it up.
        /// The result is intentionally independent of frame time because mouse input is already a per-frame delta.
        /// </summary>
        public static Vector2 GetMouseLookDelta(Vector2 pixelDelta, float sensitivityDegreesPerPixel)
        {
            float sensitivity = Mathf.Max(0f, sensitivityDegreesPerPixel);
            return new Vector2(pixelDelta.x * sensitivity, pixelDelta.y * sensitivity);
        }

        /// <summary>
        /// Converts a normalized gamepad right-stick value to local gimbal-angle deltas.
        /// Positive screen X moves the EO view right and positive screen Y moves it up.
        /// </summary>
        public static Vector2 GetGamepadLookDelta(Vector2 stickValue, float speedDegreesPerSecond, float deltaTime)
        {
            float speed = Mathf.Max(0f, speedDegreesPerSecond);
            return new Vector2(-stickValue.x * speed, stickValue.y * speed) * Mathf.Max(0f, deltaTime);
        }

        /// <summary>
        /// Converts one mouse-wheel sample to a bounded FOV delta. Any positive wheel sample zooms in;
        /// any negative sample zooms out. The magnitude is capped to one standard wheel step.
        /// </summary>
        public static float GetMouseWheelFovDelta(float wheelInput, float fovStepDegrees)
        {
            float step = Mathf.Max(0f, fovStepDegrees);
            if (Mathf.Approximately(wheelInput, 0f))
            {
                return 0f;
            }

            return -Mathf.Sign(wheelInput) * step;
        }

        /// <summary>
        /// Converts a continuous gamepad zoom value to a frame-rate independent FOV delta.
        /// </summary>
        public static float GetGamepadZoomDelta(float zoomInput, float speedDegreesPerSecond, float deltaTime)
        {
            return -zoomInput * Mathf.Max(0f, speedDegreesPerSecond) * Mathf.Max(0f, deltaTime);
        }

        public static Vector3 GetPhysicalForward(Quaternion aircraftRotation, float yawDegrees, float pitchDegrees)
        {
            Quaternion localLook = Quaternion.Euler(pitchDegrees, yawDegrees, 0f);
            return (aircraftRotation * localLook * Vector3.back).normalized;
        }
    }
}
