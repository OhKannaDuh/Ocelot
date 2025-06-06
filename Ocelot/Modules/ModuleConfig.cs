using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.Serialization;
using ECommons.Reflection;
using ImGuiNET;
using Ocelot.Config.Attributes;
using Ocelot.Config.Handlers;

namespace Ocelot.Modules;

[Serializable]
public abstract class ModuleConfig
{
    [IgnoreDataMember]
    public virtual string ProviderNamespace => "";

    private List<Handler>? handlers;

    private IEnumerable<Handler> GetHandlers()
    {
        if (this.handlers != null)
        {
            return this.handlers;
        }

        var handlers = new List<Handler>();
        var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var prop in props)
        {
            var configAttrs = prop.GetCustomAttributes<ConfigAttribute>(inherit: true);

            foreach (var attr in configAttrs)
            {
                Logger.Info($"Property: {prop.Name}, Attribute: {attr.GetType().Name}, Handler: {attr.GetHandler(this, attr, prop)?.GetType().Name}");
                handlers.Add(attr.GetHandler(this, attr, prop));
            }
        }

        if (props.Length == 0)
        {
            Logger.Info($"No properties found for config type: {GetType().Name}");
        }

        this.handlers = handlers;
        return handlers;
    }

    public bool Draw()
    {
        bool dirty = false;
        OcelotUI.Region($"OcelotConfig##{GetType().FullName}", () =>
        {
            var title = GetType().GetCustomAttribute<TitleAttribute>();
            if (title != null)
            {
                ImGui.TextColored(new Vector4(1f, 0.75f, 0.25f, 1f), $"{title.text}:");
            }

            var requiredPlugin = GetType().GetCustomAttribute<RequiredPluginAttribute>();
            if (requiredPlugin != null)
            {
                List<string> unloaded = [];

                foreach (var plugin in requiredPlugin.dependencies)
                {
                    if (!DalamudReflector.TryGetDalamudPlugin(plugin, out _, false, true))
                    {
                        unloaded.Add(plugin);
                    }
                }

                if (unloaded.Count > 0)
                {
                    OcelotUI.Error("The following plugins are required for this module:");
                    ImGui.SameLine();
                    ImGui.TextUnformatted(string.Join(", ", unloaded));
                    return;
                }

            }


            OcelotUI.Indent(16, () =>
            {
                foreach (var handler in GetHandlers())
                {
                    (bool handled, bool changed) = handler.Render();
                    if (handled && changed)
                    {
                        dirty = true;
                    }
                }
            });
        });

        return dirty;
    }
}
