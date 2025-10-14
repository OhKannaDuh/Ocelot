using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Lifecycle;

namespace Ocelot.Services;

public sealed class OcelotServiceCollection : IServiceCollection
{
    private readonly IServiceCollection collection;

    private readonly static Type[] HookInterfaces =
    {
        typeof(IOnLoad),
        typeof(IOnStart),
        typeof(IOnStop),
        typeof(IOnPreUpdate),
        typeof(IOnUpdate),
        typeof(IOnPostUpdate),
        typeof(IOnPreRender),
        typeof(IOnRender),
        typeof(IOnPostRender),
        typeof(IOnTerritoryChanged),
    };

    private readonly HashSet<(Type hook, Type viaService, ServiceLifetime life)> forwards = [];

    internal OcelotServiceCollection(IServiceCollection? inner = null)
    {
        collection = inner ?? new ServiceCollection();
    }

    internal void AutoDiscover()
    {
        foreach (var type in Registry.GetTypesWithAttribute<OcelotServiceAttribute>())
        {
            var attrs = type.GetCustomAttributes(typeof(OcelotServiceAttribute), false).Cast<OcelotServiceAttribute>();

            foreach (var a in attrs)
            {
                var service = a.Service ?? type;
                if (!service.IsAssignableFrom(type))
                {
                    throw new InvalidOperationException($"{service.FullName} is not assignable to {type.FullName}");
                }

                Add(new ServiceDescriptor(a.Service ?? type, type, a.Lifetime));
            }
        }
    }

    private void AutoWire(ServiceDescriptor item)
    {
        var type = item.ImplementationType ?? item.ImplementationInstance?.GetType() ?? null;

        if (type is null)
        {
            return;
        }

        var serviceType = item.ServiceType;

        foreach (var hook in HookInterfaces)
        {
            if (!hook.IsAssignableFrom(type))
            {
                continue;
            }

            if (hook == serviceType)
            {
                continue;
            }

            if (!forwards.Add((hook, serviceType, item.Lifetime)))
            {
                continue;
            }

            collection.Add(new ServiceDescriptor(hook, sp => sp.GetRequiredService(serviceType), item.Lifetime));
        }
    }

    internal ServiceProvider Build()
    {
        return collection.BuildServiceProvider(true);
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
        collection.Add(item);
        AutoWire(item);
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
        AutoWire(item);
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
            AutoWire(value);
        }
    }
}
