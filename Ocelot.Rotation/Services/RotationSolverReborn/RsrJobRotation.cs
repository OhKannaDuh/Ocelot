using Ocelot.Ipc.RotationSolverReborn;

namespace Ocelot.Rotation.Services.RotationSolverReborn;

public sealed class RsrJobRotation(IRotationSolverRebornIpc ipc) : IJobRotationBackend
{
    public JobRotationBackendKind Kind => JobRotationBackendKind.RotationSolverReborn;

    public void Prepare(JobRotationSessionOptions options)
    {
        _ = options;
        Disable();
    }

    public void Enable(CombatActivity activity)
    {
        ipc.ChangeOperatingMode(RSRStateCommandType.Henched);
    }

    public void Disable() => ipc.ChangeOperatingMode(RSRStateCommandType.Off);

    public void Refresh()
    {
    }

    public void Teardown() => Disable();
}
