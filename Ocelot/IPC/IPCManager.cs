using System;
using System.Collections.Generic;
using System.Linq;

namespace Ocelot.IPC;

public class IPCManager
{
    private readonly List<IPCProvider> providers = new();

    public IReadOnlyList<IPCProvider> Providers => providers.AsReadOnly();

    public void Initialize()
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

    public T GetProvider<T>() where T : IPCProvider
    {
        var provider = providers.OfType<T>().FirstOrDefault();
        if (provider == null)
        {
            throw new UnableToLoadIpcProviderException($"IPC provider of type {typeof(T).Name} was not found.");
        }

        return provider;
    }

    public bool TryGetProvider<T>(out T? provider) where T : IPCProvider
    {
        try
        {
            provider = GetProvider<T>();
            return provider.IsReady();
        }
        catch (UnableToLoadIpcProviderException ex)
        {
            Logger.Error(ex.Message);
            provider = null;
            return false;
        }
    }
}
