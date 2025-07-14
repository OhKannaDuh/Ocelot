using ECommons.EzIpcManager;
using ECommons.Reflection;

namespace Ocelot.IPC;

public abstract class IPCSubscriber
{
    protected readonly string identifier;

    public IPCSubscriber(string identifier)
    {
        this.identifier = identifier;
        EzIPC.Init(this, identifier);
    }

    public bool IsReady()
    {
        return DalamudReflector.TryGetDalamudPlugin(identifier, out _, false, true);
    }
}
