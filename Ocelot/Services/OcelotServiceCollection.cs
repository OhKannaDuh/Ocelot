using System.Collections;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Ocelot.Services;

public sealed class OcelotServiceCollection : IServiceCollection
{
    private readonly IServiceCollection collection;

    private readonly List<Type> HookInterfaces = [];

    private readonly HashSet<(Type hook, Type viaService, ServiceLifetime life)> forwards = [];

    internal OcelotServiceCollection(IServiceCollection? inner = null)
    {
        collection = inner ?? new ServiceCollection();

        foreach (var type in Registry.GetInterfaceTypesWithAttribute<OcelotAutoWireAttribute>())
        {
            HookInterfaces.Add(type);
        }
    }

    // Each class is built once under its own type and the interface points at it; forwarding the class
    // to the interface returned the last-registered implementation when an interface had several.
    private void AddWired(ServiceDescriptor item)
    {
        var serviceType = item.ServiceType;
        if (item.ImplementationType is not { } implType
            || implType == serviceType
            || serviceType.IsGenericTypeDefinition
            || implType.IsGenericTypeDefinition)
        {
            collection.Add(item);
            AutoWire(item.ServiceType, item.ImplementationType ?? item.ImplementationInstance?.GetType(), item.Lifetime);
            return;
        }

        if (collection.All(d => d.ServiceType != implType))
        {
            collection.Add(new ServiceDescriptor(implType, implType, item.Lifetime));
        }

        collection.Add(new ServiceDescriptor(serviceType, sp => sp.GetRequiredService(implType), item.Lifetime));
        AutoWire(serviceType, implType, item.Lifetime);
    }

    private void AutoWire(Type serviceType, Type? implType, ServiceLifetime lifetime)
    {
        if (implType is null || serviceType.IsGenericTypeDefinition || implType.IsGenericTypeDefinition)
        {
            return;
        }

        if (implType != serviceType && collection.All(d => d.ServiceType != implType))
        {
            collection.Add(new ServiceDescriptor(
                implType,
                sp => sp.GetRequiredService(serviceType),
                lifetime));
        }

        foreach (var hook in HookInterfaces.Distinct())
        {
            if (!hook.IsAssignableFrom(implType))
            {
                continue;
            }

            if (hook == serviceType)
            {
                continue;
            }

            if (!forwards.Add((hook, implType, lifetime)))
            {
                continue;
            }

            collection.Add(new ServiceDescriptor(
                hook,
                sp =>
                {
                    var obj = sp.GetRequiredService(implType);
                    if (!hook.IsInstanceOfType(obj))
                    {
                        throw new InvalidOperationException(
                            $"AutoWire produced {obj.GetType().FullName} for hook {hook.FullName} via {implType.FullName}");
                    }

                    return obj;
                },
                lifetime));
        }
    }


    internal ServiceProvider Build()
    {
        return collection.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true,
        });
    }

    public IEnumerator<ServiceDescriptor> GetEnumerator()
    {
        return collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(ServiceDescriptor item)
    {
        AddWired(item);
    }

    public void Clear()
    {
        collection.Clear();
        forwards.Clear();
    }

    public bool Contains(ServiceDescriptor item)
    {
        return collection.Contains(item);
    }

    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    {
        collection.CopyTo(array, arrayIndex);
    }

    public bool Remove(ServiceDescriptor item)
    {
        return collection.Remove(item);
    }

    public int Count
    {
        get => collection.Count;
    }

    public bool IsReadOnly
    {
        get => collection.IsReadOnly;
    }

    public int IndexOf(ServiceDescriptor item)
    {
        return collection.IndexOf(item);
    }

    public void Insert(int index, ServiceDescriptor item)
    {
        collection.Insert(index, item);
        AutoWire(item.ServiceType, item.ImplementationType ?? item.ImplementationInstance?.GetType(), item.Lifetime);
    }

    public void RemoveAt(int index)
    {
        collection.RemoveAt(index);
    }

    public ServiceDescriptor this[int index]
    {
        get => collection[index];

        set
        {
            collection[index] = value;
            AutoWire(value.ServiceType, value.ImplementationType ?? value.ImplementationInstance?.GetType(), value.Lifetime);
        }
    }
}
