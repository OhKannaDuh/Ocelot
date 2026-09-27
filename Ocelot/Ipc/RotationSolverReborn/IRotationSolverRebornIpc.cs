namespace Ocelot.Ipc.RotationSolverReborn;

public interface IRotationSolverRebornIpc
{
    /// <summary>True when RSR has registered ChangeOperatingMode (loaded, including Dev Mode / sideload).</summary>
    bool IsAvailable { get; }

    /// <returns>True when RSR accepted the mode change.</returns>
    bool ChangeOperatingMode(RSRStateCommandType command);
}
