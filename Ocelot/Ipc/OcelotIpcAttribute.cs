using System;

namespace Ocelot.Ipc;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class OcelotIpcAttribute(string internalName) : Attribute
{
    public readonly string InternalName = internalName;
}
