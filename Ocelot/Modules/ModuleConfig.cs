using System.Collections.Generic;
using System.Reflection;
using Ocelot.Config.Attributes;
using Ocelot.Config.Handlers;

namespace Ocelot.Modules;

public abstract class ModuleConfig
{
    public virtual string ProviderNamespace => "";

    private IEnumerable<Handler> GetHandlers()
    {
        var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var prop in props)
        {
            var configAttrs = prop.GetCustomAttributes<ConfigAttribute>(inherit: true);

            foreach (var attr in configAttrs)
            {
                yield return attr.GetHandler(this);
            }
        }
    }

    public bool Draw()
    {
        bool dirty = false;
        foreach (var handler in GetHandlers())
        {
            (bool handled, bool changed) = handler.Render();
            if (handled && changed)
            {
                dirty = true;
            }
        }

        return dirty;
    }
}
