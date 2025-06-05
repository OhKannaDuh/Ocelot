using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;

namespace Ocelot.IPC;

public class IPCManager
{
    private readonly List<IPCProvider> providers = new();

    public IReadOnlyList<IPCProvider> Providers => providers.AsReadOnly();

    public void Initialze()
    {
        foreach (var type in Registry.GetTypesImplementing<IPCProvider>())
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor != null && Activator.CreateInstance(type) is IPCProvider instance)
            {
                Logger.Info($"Registered IPCProvider: {type.FullName}");
                providers.Add(instance);
            }
        }
    }

    public T? GetProvider<T>() where T : IPCProvider => providers.OfType<T>().FirstOrDefault();

    public bool TryGetProvider<T>(out T? provider) where T : IPCProvider
    {
        provider = GetProvider<T>();
        if (provider == null)
        {
            return false;
        }

        return provider.IsReady();
    }
}
