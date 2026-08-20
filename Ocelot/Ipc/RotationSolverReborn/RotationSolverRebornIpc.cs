using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;

namespace Ocelot.Ipc.RotationSolverReborn;

public class RotationSolverRebornIpc(IDalamudPluginInterface plugin) : IRotationSolverRebornIpc
{
    private readonly ICallGateSubscriber<byte, object> changeOperatingMode = plugin
        .GetIpcSubscriber<byte, object>("RotationSolverReborn.ChangeOperatingMode");

    public bool IsAvailable
    {
        get
        {
            try
            {
                return changeOperatingMode.HasFunction;
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

        changeOperatingMode.InvokeAction((byte)command);
    }
}
