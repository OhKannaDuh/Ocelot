using System;

namespace Ocelot.Services;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class OcelotServiceAttribute(Type serviceType, ServiceLifetime lifetime = ServiceLifetime.Singleton, bool replaceExisting = false) : Attribute
{
    public Type ServiceType { get; } = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

    public ServiceLifetime Lifetime { get; } = lifetime;

    public bool ReplaceExisting { get; } = replaceExisting;
}
