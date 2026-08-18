using Ocelot.Ipc.RotationSolverReborn;

namespace Ocelot.Rotation.Services.RotationSolverReborn;

public sealed class RsrJobRotation(IRotationSolverRebornIpc ipc) : IJobRotationBackend
{
    public JobRotationBackendKind Kind => JobRotationBackendKind.RotationSolverReborn;

    private RSRStateCommandType? armed;

    public void Prepare(JobRotationSessionOptions options)
    {
        _ = options;
        Disable();
    }

    public void Enable(CombatActivity activity)
    {
        _ = activity;
        SetMode(RSRStateCommandType.Henched);
    }

    public void Disable() => SetMode(RSRStateCommandType.Off);

    public void Refresh()
    {
    }

    public void Teardown()
    {
        Disable();
        armed = null;
    }

    private void SetMode(RSRStateCommandType mode)
    {
        if (armed == mode)
        {
            return;
        }

        ipc.ChangeOperatingMode(mode);
        armed = mode;
    }
}
