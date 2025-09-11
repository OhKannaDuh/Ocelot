using System;
using System.Collections.Generic;

namespace Ocelot.Services;

public class ServiceChangedEventContext(ServiceChangeKind kind, Type serviceType, IReadOnlyList<ServiceDescriptor> added, IReadOnlyList<ServiceDescriptor> removed, IReadOnlyList<ServiceDescriptor> current) : EventArgs
{
    public ServiceChangeKind Kind { get; } = kind;

    public Type ServiceType { get; } = serviceType;

    public IReadOnlyList<ServiceDescriptor> Added { get; } = added;

    public IReadOnlyList<ServiceDescriptor> Removed { get; } = removed;

    public IReadOnlyList<ServiceDescriptor> Current { get; } = current;
}
