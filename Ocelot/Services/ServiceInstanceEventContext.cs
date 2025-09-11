using System;

namespace Ocelot.Services;

public sealed class ServiceInstanceEventContext(Type serviceType, ServiceDescriptor descriptor, object instance, bool isSingleton)
    : EventArgs
{
    public Type ServiceType { get; } = serviceType;

    public ServiceDescriptor Descriptor { get; } = descriptor;

    public object Instance { get; } = instance;

    public bool IsSingleton { get; } = isSingleton;

    public bool FromFactory {
        get => Descriptor.Factory is not null;
    }
}
