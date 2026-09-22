namespace MertKaan.UAVSimulator.CameraSystem
{
    public enum CameraMode
    {
        Chase = 0,
        EO = 1
    }

    public readonly struct CameraModeSnapshot
    {
        public CameraMode Mode { get; }
        public bool IsChaseActive => Mode == CameraMode.Chase;
        public bool IsEOActive => Mode == CameraMode.EO;

        public CameraModeSnapshot(CameraMode mode)
        {
            Mode = mode;
        }
    }
}
