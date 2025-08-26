using System;
using System.Collections.Generic;
using System.Linq;
using Ocelot.Modules;

namespace Ocelot.IPC;

public class IPCManager : IDisposable
{
    private readonly Dictionary<string, IPCSubscriber> subscribers = [];

    public IReadOnlyList<IPCSubscriber> Subscribers
    {
        get => subscribers.Values.ToList();
    }

    private readonly Dictionary<string, object> providers = [];

    public IReadOnlyList<object> Providers
    {
        get => providers.Values.ToList();
    }

    public void Initialize()
    {
        foreach (var type in Registry.GetTypesImplementing<IPCSubscriber>())
        {
            var key = type.FullName ?? type.Name;
            if (subscribers.ContainsKey(key))
            {
                continue;
            }

            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null || Activator.CreateInstance(type) is not IPCSubscriber instance)
            {
                continue;
            }

            Logger.Info($"Registered IPCSubscriber: {key}");
            subscribers.Add(key, instance);
        }

        foreach (var type in Registry.GetTypesWithAttribute<OcelotIPCAttribute>())
        {
            var key = type.FullName ?? type.Name;
            if (subscribers.ContainsKey(key))
            {
                continue;
            }

            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null || Activator.CreateInstance(type) is not { } instance)
            {
                continue;
            }

            Logger.Info($"Registered OcelotIPC: {key}");
            providers.Add(key, instance);
        }
    }

    public void AddProvider(object provider)
    {
        var type = provider.GetType();
        providers.Add(type.FullName ?? type.Name, provider);
    }

    public IPCSubscriber GetSubscriber(Type type)
    {
        var subscriber = Subscribers.FirstOrDefault(type.IsInstanceOfType);
        if (subscriber == null)
        {
            throw new UnableToLoadIpcSubscriberException($"IPC subscriber of type {type.Name} was not found.");
        }

        if (!subscriber.IsReady())
        {
            throw new UnableToLoadIpcSubscriberException($"IPC subscriber of type {type.Name} was not ready.");
        }

        return subscriber;
    }

    public T GetSubscriber<T>() where T : IPCSubscriber
    {
        var subscriber = Subscribers.OfType<T>().FirstOrDefault();
        if (subscriber == null)
        {
            throw new UnableToLoadIpcSubscriberException($"IPC subscriber of type {typeof(T).Name} was not found.");
        }

        if (!subscriber.IsReady())
        {
            throw new UnableToLoadIpcSubscriberException($"IPC subscriber of type {typeof(T).Name} was not ready.");
        }

        return subscriber;
    }

    public bool TryGetSubscriber<T>(out T? subscriber) where T : IPCSubscriber
    {
        try
        {
            subscriber = GetSubscriber<T>();
            return subscriber.IsReady();
        }
        catch (UnableToLoadIpcSubscriberException ex)
        {
            Logger.Error(ex.Message);
            subscriber = null;
            return false;
        }
    }

    public void Dispose()
    {
        providers.Clear();
        subscribers.Clear();
    }
}
