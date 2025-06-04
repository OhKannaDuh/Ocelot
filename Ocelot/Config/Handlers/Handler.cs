using System;
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

    public Handler(ModuleConfig self, ConfigAttribute attribute)
    {
        this.self = self;
        this.attribute = attribute;
    }

    protected RenderContext GetContext()
    {
        var property = self.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(p => p.GetCustomAttributes<ConfigAttribute>(true).Any(attr => attr == attribute));

        if (property == null)
        {
            throw new InvalidOperationException("No valid property for attribute found.");
        }

        return new RenderContext(property, type, self);
    }

    public (bool handled, bool changed) Render()
    {
        RenderContext context = GetContext();

        if (!context.IsValid() || !context.ShoulRender())
        {
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
