using System;

namespace Ocelot.Config.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RenderIfAttribute : Attribute
{
    public string[] dependencies { get; }

    public RenderIfAttribute(params string[] dependencies)
    {
        this.dependencies = dependencies;
    }
}
