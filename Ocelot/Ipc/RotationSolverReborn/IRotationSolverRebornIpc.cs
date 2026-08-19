namespace Ocelot.Ipc.RotationSolverReborn;

public interface IRotationSolverRebornIpc
{
    /// <summary>True when RSR has registered IPC (loaded, including Dev Mode / sideload).</summary>
    bool IsAvailable { get; }

    void ChangeOperatingMode(RSRStateCommandType command);
}
