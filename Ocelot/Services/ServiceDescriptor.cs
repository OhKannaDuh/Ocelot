using System;

namespace Ocelot.Services;

public sealed class ServiceDescriptor
{
    public Type ServiceType { get; }

    public Type? ImplementationType { get; }

    public Type? AliasImplementationType { get; private set; }

    public Func<ServiceContainer, object>? Factory { get; }

    public object? Instance { get; }

    public ServiceLifetime Lifetime { get; }

    public bool IsPrimary { get; }

    private ServiceDescriptor(
        Type serviceType,
        Type? implementationType,
        ServiceLifetime lifetime,
        Func<ServiceContainer, object>? factory,
        object? instance,
        bool isPrimary)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
        Lifetime = lifetime;
        Factory = factory;
        Instance = instance;
        IsPrimary = isPrimary;
    }

    public static ServiceDescriptor Impl(Type serviceType, Type implementationType, ServiceLifetime lifetime, bool isPrimary = false)
    {
        return new ServiceDescriptor(serviceType, implementationType, lifetime, null, null, isPrimary);
    }

    public static ServiceDescriptor CreateFactory(Type serviceType, Func<ServiceContainer, object> factory, ServiceLifetime lifetime, bool isPrimary = false)
    {
        return new ServiceDescriptor(serviceType, null, lifetime, factory, null, isPrimary);
    }

    public static ServiceDescriptor CreateInstance(Type serviceType, object instance, bool isPrimary = false)
    {
        return new ServiceDescriptor(serviceType, null, ServiceLifetime.Singleton, null, instance, isPrimary);
    }

    public static ServiceDescriptor Alias(Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        var sd = CreateFactory(
            serviceType,
            c => c.Get(implementationType),
            lifetime,
            false
        );

        sd.AliasImplementationType = implementationType;
        return sd;
    }
}
