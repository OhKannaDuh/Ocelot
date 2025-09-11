using System;
using System.Collections.Concurrent;
using ECommons;
using Ocelot.Modules;
using Ocelot.Windows;

namespace Ocelot.Services;

public static class OcelotServices
{
    private static readonly Lazy<ServiceContainer> Lazy = new(() => new ServiceContainer());

    public static ServiceContainer Container {
        get => Lazy.Value;
    }

    public static void Initialize(OcelotPlugin plugin, OcelotConfig config)
    {
        // Services
        var services = Registry.GetTypesWithAttribute<OcelotServiceAttribute>();
        Ocelot.Logger.Info("[ServiceContainer]: Registering services");
        foreach (var service in services)
        {
            Container.RegisterDiscoveredType(service);
        }

        // Modules
        var modules = Registry.GetTypesWithAttribute<OcelotModuleAttribute>();
        Ocelot.Logger.Info("[ServiceContainer]: Registering modules");
        foreach (var module in modules)
        {
            var instance = (IModule)Activator.CreateInstance(module, plugin, config)!;
            var descriptor = ServiceDescriptor.CreateInstance(module, instance);
            Register(descriptor);
        }

        // Windows
        var windows = Registry.GetTypesWithAttribute<OcelotWindowAttribute>();
        Ocelot.Logger.Info("[ServiceContainer]: Registering windows");
        foreach (var window in windows)
        {
            var instance = Activator.CreateInstance(window, plugin, config) as OcelotWindow;
            if (instance == null)
            {
                continue;
            }

            var descriptor = ServiceDescriptor.CreateInstance(window, instance);
            Register(descriptor);
        }

        if (Registry.GetTypesWithAttribute<OcelotMainWindowAttribute>().TryGetFirst(out var mainWindowType))
        {
            if (Activator.CreateInstance(mainWindowType, plugin, config) is OcelotMainWindow instance)
            {
                Container.Register(ServiceDescriptor.CreateInstance(mainWindowType, instance));
                Container.TryAdd(ServiceDescriptor.Alias(typeof(OcelotMainWindow), mainWindowType, ServiceLifetime.Singleton));
                Container.TryAdd(ServiceDescriptor.Alias(typeof(OcelotWindow), mainWindowType, ServiceLifetime.Singleton));
            }
        }

        if (Registry.GetTypesWithAttribute<OcelotConfigWindowAttribute>().TryGetFirst(out var configWindowType))
        {
            if (Activator.CreateInstance(configWindowType, plugin, config) is OcelotConfigWindow instance)
            {
                Container.Register(ServiceDescriptor.CreateInstance(configWindowType, instance));
                Container.TryAdd(ServiceDescriptor.Alias(typeof(OcelotConfigWindow), configWindowType, ServiceLifetime.Singleton));
                Container.TryAdd(ServiceDescriptor.Alias(typeof(OcelotWindow), configWindowType, ServiceLifetime.Singleton));
            }
        }


        Container.OnServiceChanged += FlushCache;
    }

    private static readonly ConcurrentDictionary<Type, Lazy<object>> Cache = new();

    public static T Get<T>()
    {
        return Container.Get<T>();
    }

    public static T GetCached<T>()
    {
        if (!Cache.ContainsKey(typeof(T)))
        {
            Cache[typeof(T)] = new Lazy<object>(() => Container.Get<T>() ?? throw new InvalidOperationException());
        }

        return (T)Cache[typeof(T)].Value;
    }

    public static T? GetOptional<T>() where T : class
    {
        return Container.GetOptional<T>();
    }

    public static void Register(ServiceDescriptor descriptor, bool replaceExisting = false)
    {
        Container.Register(descriptor, replaceExisting);
    }

    public static bool TryAdd(ServiceDescriptor descriptor)
    {
        return Container.TryAdd(descriptor);
    }

    private static void FlushCache(object? sender, ServiceChangedEventContext e)
    {
        foreach (var kv in Cache.Values)
        {
            if (kv is { IsValueCreated: true, Value: IDisposable d })
            {
                d.Dispose();
            }
        }

        Cache.Clear();
    }


    public static void Dispose()
    {
        Container.OnServiceChanged -= FlushCache;
        Container.Dispose();
    }
}
