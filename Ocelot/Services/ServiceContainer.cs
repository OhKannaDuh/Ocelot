using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Ocelot.Intents;
using Ocelot.Services.Pathfinding;

namespace Ocelot.Services;

public sealed class ServiceContainer : IDisposable
{
    private readonly Dictionary<Type, List<ServiceDescriptor>> map = new();

    private readonly ConcurrentDictionary<(Type serviceType, ServiceDescriptor desc), Lazy<object>> singletons = new();

    private readonly Lock gate = new();

    private bool disposed;

    public event EventHandler<ServiceChangedEventContext>? OnServiceChanged;

    public event EventHandler<ServiceInstanceEventContext>? OnInstanceCreated;

    public event EventHandler<ServiceInstanceEventContext>? OnInstanceDisposed;

    internal void RegisterDiscoveredType(Type implType)
    {
        ArgumentNullException.ThrowIfNull(implType);

        if (implType.IsAbstract || implType.IsInterface)
        {
            return;
        }

        var svcAttrs = implType.GetCustomAttributes(typeof(OcelotServiceAttribute), false).Cast<OcelotServiceAttribute>();

        foreach (var a in svcAttrs)
        {
            var serviceType = a.ServiceType ?? implType;
            if (!serviceType.IsAssignableFrom(implType))
            {
                throw new InvalidOperationException(
                    $"[OcelotService] on {implType.FullName} declares {serviceType.FullName}, " +
                    $"but {implType.Name} is not assignable to that service.");
            }

            var descriptor = ServiceDescriptor.Impl(
                serviceType,
                implType,
                a.Lifetime,
                false
            );

            Register(descriptor, a.ReplaceExisting);
        }

        var facAttrs = implType.GetCustomAttributes(typeof(OcelotFactoryAttribute), false)
            .Cast<OcelotFactoryAttribute>();

        foreach (var fa in facAttrs)
        {
            if (!typeof(IOcelotFactory).IsAssignableFrom(fa.FactoryType))
                throw new InvalidOperationException($"FactoryType {fa.FactoryType.FullName} must implement IOcelotFactory.");

            var descriptor = ServiceDescriptor.CreateFactory(
                fa.ServiceType,
                c => {
                    var factory = (IOcelotFactory)Activator.CreateInstance(fa.FactoryType)!;
                    return factory.Create(c);
                },
                fa.Lifetime,
                false
            );

            Register(descriptor, false);
        }
    }

    private static bool IsIntentInterface(Type t)
    {
        if (t.IsGenericType)
        {
            return t.GetGenericTypeDefinition().GetCustomAttributes(typeof(IntentAttribute), false).Any();
        }

        return t.GetCustomAttributes(typeof(IntentAttribute), false).Any();
    }

    private static ServiceLifetime IntentLifetime(Type intentInterface)
    {
        var attr = (IntentAttribute?)(intentInterface.IsGenericType
                                          ? Attribute.GetCustomAttribute(intentInterface.GetGenericTypeDefinition(), typeof(IntentAttribute))
                                          : Attribute.GetCustomAttribute(intentInterface, typeof(IntentAttribute)));


        return attr?.Lifetime ?? ServiceLifetime.Singleton;
    }

    private void AutoMapIntents(ServiceDescriptor descriptor)
    {
        if (descriptor.AliasImplementationType != null)
        {
            return;
        }

        var implType = descriptor.ImplementationType ?? descriptor.Instance?.GetType();
        if (implType == null || implType.IsAbstract || implType.IsInterface)
        {
            return;
        }

        var hasAnchor =
            map.TryGetValue(implType, out var existingForImpl) &&
            existingForImpl.Any(d =>
                d.ImplementationType == implType ||
                d.Instance != null && d.Instance.GetType() == implType);

        if (!hasAnchor)
        {
            var anchor = ServiceDescriptor.Impl(implType, implType, ServiceLifetime.Singleton);
            AddCore(anchor, false);
        }

        foreach (var iface in implType.GetInterfaces())
        {
            if (!IsIntentInterface(iface))
            {
                continue;
            }

            if (map.TryGetValue(iface, out var listForIface) && listForIface.Any(x => x.AliasImplementationType == implType))
            {
                continue;
            }

            var lifetime = IntentLifetime(iface);
            var alias = ServiceDescriptor.Alias(iface, implType, lifetime);
            AddCore(alias, false);
        }
    }


    public void Register(ServiceDescriptor descriptor, bool replaceExisting = false)
    {
        Ocelot.Logger.Info($"[ServiceContainer]: Registering {descriptor.ServiceType.FullName}");
        ObjectDisposedException.ThrowIf(disposed, nameof(ServiceContainer));
        ArgumentNullException.ThrowIfNull(descriptor);

        lock (gate)
        {
            AddCore(descriptor, replaceExisting);
            AutoMapIntents(descriptor);
        }
    }

    public bool TryAdd(ServiceDescriptor descriptor)
    {
        ObjectDisposedException.ThrowIf(disposed, nameof(ServiceContainer));
        ArgumentNullException.ThrowIfNull(descriptor);

        lock (gate)
        {
            if (!map.TryGetValue(descriptor.ServiceType, out var list))
            {
                map[descriptor.ServiceType] = new List<ServiceDescriptor> { descriptor };
                PrimeSingleton(descriptor);
                return true;
            }

            if (list.Any(x =>
                    x.ImplementationType == descriptor.ImplementationType &&
                    x.Instance == descriptor.Instance &&
                    x.Factory == descriptor.Factory &&
                    x.AliasImplementationType == descriptor.AliasImplementationType))
            {
                return false;
            }

            list.Add(descriptor);

            var svcType = descriptor.ServiceType;
            var snapshot = list.ToList();

            ThreadPool.UnsafeQueueUserWorkItem(_ => {
                RaiseServiceChanged(new ServiceChangedEventContext(
                    ServiceChangeKind.Added, svcType,
                    [descriptor],
                    [],
                    snapshot
                ));
            }, null);

            PrimeSingleton(descriptor);

            return true;
        }
    }

    public bool Remove(Type serviceType)
    {
        ObjectDisposedException.ThrowIf(disposed, nameof(ServiceContainer));
        ArgumentNullException.ThrowIfNull(serviceType);

        lock (gate)
        {
            if (!map.Remove(serviceType, out var removed))
            {
                return false;
            }

            foreach (var d in removed)
            {
                DisposeSingletonIfExists((serviceType, d));
            }

            var removedCopy = removed.ToList();
            var current = map.TryGetValue(serviceType, out var after) ? after.ToList() : new List<ServiceDescriptor>();

            ThreadPool.UnsafeQueueUserWorkItem(_ => {
                RaiseServiceChanged(new ServiceChangedEventContext(
                    ServiceChangeKind.Removed, serviceType,
                    [],
                    removedCopy,
                    current
                ));
            }, null);

            return true;
        }
    }

    public void AddSingleton<TService, TImpl>() where TImpl : class, TService
    {
        Register(ServiceDescriptor.Impl(typeof(TService), typeof(TImpl), ServiceLifetime.Singleton));
    }

    public void AddSingleton<TService>(TService instance) where TService : class
    {
        Register(ServiceDescriptor.CreateInstance(typeof(TService), instance!));
    }

    public void AddTransient<TService, TImpl>() where TImpl : class, TService
    {
        Register(ServiceDescriptor.Impl(typeof(TService), typeof(TImpl), ServiceLifetime.Transient));
    }

    public T Get<T>()
    {
        return (T)Get(typeof(T));
    }

    public object Get(Type serviceType)
    {
        ObjectDisposedException.ThrowIf(disposed, nameof(ServiceContainer));

        if (TryResolveRegistered(serviceType, out var resolved))
        {
            return resolved!;
        }

        if (!serviceType.IsAbstract && !serviceType.IsInterface)
        {
            return CreateForConcrete(serviceType);
        }

        var impl = FindSingleImplementation(serviceType);
        if (impl != null)
        {
            var sd = ServiceDescriptor.Impl(serviceType, impl, ServiceLifetime.Singleton);
            Register(sd);

            return Get(serviceType);
        }

        throw new InvalidOperationException($"Service not registered and no unique implementation found for {serviceType.FullName}");
    }

    public bool TryGet(Type serviceType, out object? instance)
    {
        try
        {
            instance = Get(serviceType);
            return true;
        }
        catch
        {
            instance = null;
            return false;
        }
    }

    public bool TryGet<T>(out T? instance) where T : class
    {
        var ok = TryGet(typeof(T), out var o);
        instance = (T?)o;
        return ok;
    }

    public T? GetOptional<T>() where T : class
    {
        return TryGet<T>(out var t) ? t : null;
    }

    public IEnumerable<T> GetAll<T>()
    {
        ObjectDisposedException.ThrowIf(disposed, nameof(ServiceContainer));

        var t = typeof(T);
        var results = new List<T>();

        List<ServiceDescriptor> snapshot;
        lock (gate)
        {
            snapshot = map.TryGetValue(t, out var list) ? list.ToList() : new List<ServiceDescriptor>();
            foreach (var kv in map)
            {
                if (kv.Key == t) continue;
                foreach (var d in kv.Value)
                {
                    var impl = d.ImplementationType;
                    if (impl is not null && t.IsAssignableFrom(impl))
                        snapshot.Add(d);
                }
            }
        }

        foreach (var d in snapshot)
        {
            results.Add((T)ResolveByDescriptor(d, d.ServiceType));
        }

        return results;
    }

    public bool TryGetAll<T>(out IReadOnlyList<T> items)
    {
        try
        {
            items = GetAll<T>().ToList();
            return true;
        }
        catch
        {
            items = [];
            return false;
        }
    }

    private void AddCore(ServiceDescriptor descriptor, bool replaceExisting)
    {
        var previous = Array.Empty<ServiceDescriptor>().ToList();

        if (!map.TryGetValue(descriptor.ServiceType, out var list))
        {
            list = [];
            map[descriptor.ServiceType] = list;
        }

        if (replaceExisting && list.Count > 0)
        {
            previous = list.ToList();
            foreach (var existing in list.ToArray())
            {
                DisposeSingletonIfExists((descriptor.ServiceType, existing));
            }

            list.Clear();
        }

        list.Add(descriptor);
        PrimeSingleton(descriptor);

        var currentSnapshot = list.ToList();

        ThreadPool.UnsafeQueueUserWorkItem(_ => {
            var kind = previous.Count > 0 ? ServiceChangeKind.Replaced : ServiceChangeKind.Added;
            RaiseServiceChanged(new ServiceChangedEventContext(
                kind,
                descriptor.ServiceType,
                [descriptor],
                previous,
                currentSnapshot
            ));
        }, null);
    }

    private void PrimeSingleton(ServiceDescriptor d)
    {
        if (d.Lifetime == ServiceLifetime.Singleton)
        {
            var key = (d.ServiceType, d);
            singletons.GetOrAdd(key, _ => new Lazy<object>(() => Create(d, new Stack<Type>()), true));
        }
    }

    private object ResolveByDescriptor(ServiceDescriptor d, Type serviceType)
    {
        if (d.Lifetime == ServiceLifetime.Singleton)
        {
            var instance = singletons[(serviceType, d)].Value;
            return instance;
        }

        var created = Create(d, new Stack<Type>());
        RaiseInstanceCreated(new ServiceInstanceEventContext(serviceType, d, created, false));
        return created;
    }

    private bool TryResolveRegistered(Type serviceType, out object? result)
    {
        List<ServiceDescriptor>? list;
        lock (gate)
        {
            map.TryGetValue(serviceType, out list);
        }

        if (list is null || list.Count == 0)
        {
            result = null;
            return false;
        }

        var chosen = list.FirstOrDefault(x => x.IsPrimary) ?? list[0];
        result = ResolveByDescriptor(chosen, serviceType);
        return true;
    }

    private Type? FindSingleImplementation(Type serviceType)
    {
        var candidates = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => {
                try { return a.GetTypes(); }
                catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t is not null)!; }
            })
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                        && serviceType.IsAssignableFrom(t!)
                        && !t!.IsGenericTypeDefinition)
            .Cast<Type>()
            .ToList();

        return candidates.Count >= 1 ? candidates[0] : null;
    }

    private object CreateForConcrete(Type concrete)
    {
        var d = ServiceDescriptor.Impl(concrete, concrete, ServiceLifetime.Transient);
        return Create(d, new Stack<Type>());
    }

    private object Create(ServiceDescriptor desc, Stack<Type> stack)
    {
        if (desc.Instance is not null) return desc.Instance;
        if (desc.Factory is not null)
        {
            var instF = desc.Factory(this);
            RaiseInstanceCreated(new ServiceInstanceEventContext(desc.ServiceType, desc, instF, desc.Lifetime == ServiceLifetime.Singleton));
            return instF;
        }

        var impl = desc.ImplementationType ?? throw new InvalidOperationException($"No factory or implementation type for {desc.ServiceType.FullName}");
        if (stack.Contains(impl))
        {
            throw new InvalidOperationException($"Circular dependency detected: {string.Join(" -> ", stack.Reverse().Select(t => t.Name))} -> {impl.Name}");
        }

        var ctors = impl.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderByDescending(c => c.GetParameters().Length);

        foreach (var ctor in ctors)
        {
            var parms = ctor.GetParameters();
            var args = new object?[parms.Length];
            var ok = true;

            stack.Push(impl);
            for (var i = 0; i < parms.Length; i++)
            {
                try { args[i] = Get(parms[i].ParameterType); }
                catch
                {
                    ok = false;
                    break;
                }
            }

            stack.Pop();

            if (ok)
            {
                var created = Activator.CreateInstance(impl, args!)!;
                RaiseInstanceCreated(new ServiceInstanceEventContext(desc.ServiceType, desc, created, desc.Lifetime == ServiceLifetime.Singleton));
                return created;
            }
        }

        throw new InvalidOperationException($"No satisfiable constructor found for {impl.FullName}");
    }

    private void RaiseServiceChanged(ServiceChangedEventContext e)
    {
        try
        {
            OnServiceChanged?.Invoke(this, e);
        }
        catch
        {
            /* don't let handlers break container */
        }
    }

    private void RaiseInstanceCreated(ServiceInstanceEventContext e)
    {
        try
        {
            OnInstanceCreated?.Invoke(this, e);
        }
        catch
        {
            /* don't let handlers break container */
        }
    }

    private void RaiseInstanceDisposed(ServiceInstanceEventContext e)
    {
        try
        {
            OnInstanceDisposed?.Invoke(this, e);
        }
        catch
        {
            /* don't let handlers break container */
        }
    }

    private void DisposeSingletonIfExists((Type serviceType, ServiceDescriptor desc) key)
    {
        if (singletons.TryRemove(key, out var lazy) && lazy is { IsValueCreated: true, Value: IDisposable d })
        {
            try
            {
                d.Dispose();
                RaiseInstanceDisposed(new ServiceInstanceEventContext(key.serviceType, key.desc, d, true));
            }
            catch
            {
                /* ignore on mutation */
            }
        }
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;

        foreach (var kv in singletons)
        {
            if (kv.Value is { IsValueCreated: true, Value: IDisposable d })
            {
                try
                {
                    d.Dispose();
                    RaiseInstanceDisposed(new ServiceInstanceEventContext(kv.Key.serviceType, kv.Key.desc, d, true));
                }
                catch
                {
                    /* ignore on teardown */
                }
            }
        }
    }
}
