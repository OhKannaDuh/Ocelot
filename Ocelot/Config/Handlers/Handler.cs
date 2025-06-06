using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace Ocelot.Config.Handlers;

public abstract class Handler
{
    protected abstract Type type { get; }

    private ModuleConfig self;

    private ConfigAttribute attribute;

    private readonly PropertyInfo property;

    public Handler(ModuleConfig self, ConfigAttribute attribute, PropertyInfo property)
    {
        this.self = self;
        this.attribute = attribute;
        this.property = property;
    }

    protected RenderContext GetContext() => new RenderContext(property, type, self);

    public bool HasUnloadedRequiredPlugins(out List<string> unloaded) => GetContext().HasUnloadedRequiredPlugins(out unloaded);

    public (bool handled, bool changed) Render()
    {
        RenderContext context = GetContext();

        if (!context.IsValid() || !context.ShouldRender())
        {
            context.LogTypes();
            return (false, false);
        }



        if (context.IsExperimental())
        {
            context.Experimental();
        }

        (bool handled, bool changed) = RenderComponent(context);

        context.Tooltip();

        return (handled, changed);
    }

    protected abstract (bool handled, bool changed) RenderComponent(RenderContext payload);
}
