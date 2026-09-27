namespace Ocelot.Ipc.RotationSolverReborn;

public enum RSRStateCommandType : byte
{
    Off,

    Auto,

    TargetOnly,

    Manual,

    AutoDuty,

    /// <summary>IPC-managed: rotation only, no targeting (Henchman / BOCCHI + BossMod AI).</summary>
    Henched,

    PvP,
}
