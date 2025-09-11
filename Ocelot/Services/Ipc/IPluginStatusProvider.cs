using System;

namespace Ocelot.Services.Ipc;

public interface IPluginStatusProvider
{
    bool IsLoaded(string internalName);

    event EventHandler<string>? PluginLoaded;

    event EventHandler<string>? PluginUnloaded;
}
