using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;

namespace Ocelot.Ipc.RotationSolverReborn;

public class RotationSolverRebornIpc(IDalamudPluginInterface plugin) : IRotationSolverRebornIpc
{
    // EzIPC registers ChangeOperatingMode as an Action (void), last generic = object.
    // HasFunction is for returning funcs and stays false here — 4.1.0.4 used that and never called Henched.
    private readonly ICallGateSubscriber<RSRStateCommandType, object> changeOperatingMode = plugin
        .GetIpcSubscriber<RSRStateCommandType, object>("RotationSolverReborn.ChangeOperatingMode");

    private readonly ICallGateSubscriber<byte, object> changeOperatingModeByte = plugin
        .GetIpcSubscriber<byte, object>("RotationSolverReborn.ChangeOperatingMode");

    public bool IsAvailable
    {
        get
        {
            try
            {
                return changeOperatingMode.HasAction || changeOperatingModeByte.HasAction;
            }
            catch
            {
                return false;
            }
        }
    }

    public bool ChangeOperatingMode(RSRStateCommandType command)
    {
        try
        {
            if (changeOperatingMode.HasAction)
            {
                // RSR Off releases a Wrath lease RSR itself took. Wrath then fails to call
                // RotationSolver.LeaseCancelled — that throws after RSR already changed mode.
                // Treat invoke as success so we do not retry Off and spam the Dalamud log.
                try
                {
                    changeOperatingMode.InvokeAction(command);
                }
                catch
                {
                }

                return true;
            }

            if (changeOperatingModeByte.HasAction)
            {
                try
                {
                    changeOperatingModeByte.InvokeAction((byte)command);
                }
                catch
                {
                }

                return true;
            }
        }
        catch
        {
        }

        return false;
    }
}
