using System;

namespace Ocelot.Services.Pathfinding;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class OcelotFactoryAttribute(Type serviceType, Type factoryType, ServiceLifetime lifetime = ServiceLifetime.Singleton) : Attribute
{
    public Type ServiceType { get; } = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

    public ServiceLifetime Lifetime { get; } = lifetime;

    public Type FactoryType { get; } = factoryType ?? throw new ArgumentNullException(nameof(factoryType));
}
