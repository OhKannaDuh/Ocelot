using ECommons.EzIpcManager;
using ECommons.Reflection;

namespace Ocelot.IPC;

public abstract class IPCProvider
{
    protected readonly string identifier;

    public IPCProvider(string identifier)
    {
        this.identifier = identifier;
        EzIPC.Init(this, identifier);
    }

    public bool IsReady()
    {
        return DalamudReflector.TryGetDalamudPlugin(identifier, out _, false, true);
    }
}
