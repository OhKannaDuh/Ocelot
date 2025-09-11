using System;
using Ocelot.Services;

namespace Ocelot.Intents;

[AttributeUsage(AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
public sealed class IntentAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }

    public IntentAttribute(ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        Lifetime = lifetime;
    }
}
