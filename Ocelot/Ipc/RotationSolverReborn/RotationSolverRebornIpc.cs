using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;

namespace Ocelot.Ipc.RotationSolverReborn;

public class RotationSolverRebornIpc(IDalamudPluginInterface plugin) : IRotationSolverRebornIpc
{
    private readonly ICallGateSubscriber<RSRStateCommandType, object> getAutoRotationState = plugin
        .GetIpcSubscriber<RSRStateCommandType, object>("RotationSolverReborn.ChangeOperatingMode");

    public void ChangeOperatingMode(RSRStateCommandType command)
    {
        getAutoRotationState.InvokeAction(command);
    }
}
