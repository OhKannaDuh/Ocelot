using System;
using System.Collections.Generic;
using System.Linq;
using Ocelot.Modules;

namespace Ocelot.IPC;

public class IPCManager
{
    private readonly List<IPCSubscriber> subscribers = new();

    public IReadOnlyList<IPCSubscriber> Subscribers
    {
        get => subscribers;
    }

    private readonly List<object> providers = new();

    public void Initialize()
    {
        foreach (var type in Registry.GetTypesImplementing<IPCSubscriber>())
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor != null && Activator.CreateInstance(type) is IPCSubscriber instance)
            {
                Logger.Info($"Registered IPCProvider: {type.FullName}");
                subscribers.Add(instance);
            }
        }

        foreach (var type in Registry.GetTypesWithAttribute<OcelotIPCAttribute>())
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null || Activator.CreateInstance(type) is not { } instance)
            {
                continue;
            }

            Logger.Info($"Registered OcelotIPC: {type.FullName}");
            providers.Add(instance);
        }
    }

    public IPCSubscriber GetProvider(Type type)
    {
        var provider = subscribers.FirstOrDefault(type.IsInstanceOfType);
        if (provider == null)
        {
            throw new UnableToLoadIpcProviderException($"IPC provider of type {type.Name} was not found.");
        }

        if (!provider.IsReady())
        {
            throw new UnableToLoadIpcProviderException($"IPC provider of type {type.Name} was not ready.");
        }

        return provider;
    }

    public T GetProvider<T>() where T : IPCSubscriber
    {
        var provider = subscribers.OfType<T>().FirstOrDefault();
        if (provider == null)
        {
            throw new UnableToLoadIpcProviderException($"IPC provider of type {typeof(T).Name} was not found.");
        }

        if (!provider.IsReady())
        {
            throw new UnableToLoadIpcProviderException($"IPC provider of type {typeof(T).Name} was not ready.");
        }

        return provider;
    }

    public bool TryGetProvider<T>(out T? provider) where T : IPCSubscriber
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
