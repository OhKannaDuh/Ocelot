using Ocelot.Ipc.RotationSolverReborn;

namespace Ocelot.Rotation.Services.RotationSolverReborn;

public class RotationSolverRebornRotationService(IRotationSolverRebornIpc ipc) : IRotationService
{
    public void EnableAutoRotation()
    {
        ipc.ChangeOperatingMode(RSRStateCommandType.Manual);
    }

    public void DisableAutoRotation()
    {
        ipc.ChangeOperatingMode(RSRStateCommandType.Off);
    }

    public void EnableSingleTarget()
    {
        // I should ask LTS or whoever about this
    }

    public void DisableSingleTarget()
    {
        // I should ask LTS or whoever about this
    }
}
