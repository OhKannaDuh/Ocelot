using System;
using ECommons.Reflection;

namespace Ocelot.Services.Ipc;

[OcelotService(typeof(IPluginStatusProvider), ServiceLifetime.Singleton)]
public class DalamudPluginStatusProvider : IPluginStatusProvider
{
    public event EventHandler<string>? PluginLoaded;

    public event EventHandler<string>? PluginUnloaded;

    public bool IsLoaded(string internalName)
    {
        return DalamudReflector.TryGetDalamudPlugin(internalName, out _, false, true);
    }
}
