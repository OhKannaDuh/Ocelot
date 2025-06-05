using ECommons.EzIpcManager;
using ECommons.Reflection;

namespace Ocelot.IPC;

public abstract class IPCProvider
{
    protected readonly string identifier;

    public IPCProvider(string identifier)
    {
        this.identifier = identifier;
        EzIPC.Init(this, identifier, SafeWrapper.AnyException);
    }

    public bool IsReady() => DalamudReflector.TryGetDalamudPlugin(identifier, out _, false, true);

}
