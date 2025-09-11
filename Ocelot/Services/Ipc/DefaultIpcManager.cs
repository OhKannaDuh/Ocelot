using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading;
using ECommons.EzIpcManager;
using Ocelot.Ipc;
using Ocelot.Services.Logger;

namespace Ocelot.Services.Ipc;

[OcelotService(typeof(IIpcManager))]
public class DefaultIpcManager : IIpcManager
{
    private static ILoggerService Logger {
        get => OcelotServices.GetCached<ILoggerService>();
    }

    private static IPluginStatusProvider PluginStatusProvider {
        get => OcelotServices.GetCached<IPluginStatusProvider>();
    }

    private readonly ConcurrentDictionary<Type, byte> initialized = new();

    private readonly Lock gate = new();

    public bool IsInitialized(Type t)
    {
        return initialized.ContainsKey(t);
    }

    public void Refresh()
    {
        Type[] providers;
        lock (gate)
        {
            providers = Registry.GetTypesWithAttribute<OcelotIpcAttribute>().ToArray();
        }

        foreach (var type in providers)
        {
            TryInit(type);
        }
    }

    private void RefreshForPlugin(string internalName)
    {
        Type[] providers;
        lock (gate)
        {
            providers = Registry.GetTypesWithAttribute<OcelotIpcAttribute>().ToArray();
        }

        foreach (var type in providers)
        {
            var attr = type.GetCustomAttribute<OcelotIpcAttribute>();
            if (attr is null)
            {
                continue;
            }


            if (!string.Equals(attr.InternalName, internalName, StringComparison.Ordinal))
            {
                continue;
            }

            TryInit(type);
        }
    }

    private void TryInit(Type type)
    {
        if (initialized.ContainsKey(type))
        {
            return;
        }

        var attr = type.GetCustomAttribute<OcelotIpcAttribute>();
        if (attr is null)
        {
            return;
        }

        var name = attr.InternalName;
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (!PluginStatusProvider.IsLoaded(name))
        {
            return;
        }

        try
        {
            EzIPC.Init(type, name, SafeWrapper.AnyException);
            initialized.TryAdd(type, 0);

            Logger.Info($"[IPC] Initialized {type.FullName} (plugin '{name}')");
        }
        catch (Exception ex)
        {
            Logger.Error($"[IPC] Failed to init {type.FullName} (plugin '{name}'): {ex.Message}");
        }
    }
}
