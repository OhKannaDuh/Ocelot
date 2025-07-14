using System;

namespace Ocelot.Modules;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class OcelotModuleAttribute : Attribute
{
    public int configOrder { get; }

    public int mainOrder { get; }

    public OcelotModuleAttribute(int configOrder = int.MaxValue, int mainOrder = int.MaxValue)
    {
        this.configOrder = configOrder;
        this.mainOrder = mainOrder;
    }
}
