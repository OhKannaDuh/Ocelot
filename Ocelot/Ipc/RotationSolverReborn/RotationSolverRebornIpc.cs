using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;

namespace Ocelot.Ipc.RotationSolverReborn;

public class RotationSolverRebornIpc(IDalamudPluginInterface plugin) : IRotationSolverRebornIpc
{
    private readonly ICallGateSubscriber<RSRStateCommandType, object> getAutoRotationState = plugin
        .GetIpcSubscriber<RSRStateCommandType, object>("RotationSolverReborn.ChangeOperatingMode");

    public bool IsAvailable
    {
        get
        {
            try
            {
                return getAutoRotationState.HasFunction;
            }
            catch
            {
                return false;
            }
        }
    }

    public void ChangeOperatingMode(RSRStateCommandType command)
    {
        if (!IsAvailable)
        {
            return;
        }

        getAutoRotationState.InvokeAction(command);
    }
}
