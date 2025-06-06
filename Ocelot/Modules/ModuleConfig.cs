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

            var error = false;
            var requiredPlugin = GetType().GetCustomAttribute<RequiredPluginAttribute>();
            if (requiredPlugin != null)
            {
                List<string> missingClass = [];
                foreach (var plugin in requiredPlugin.dependencies)
                {
                    if (!DalamudReflector.TryGetDalamudPlugin(plugin, out _, false, true))
                    {
                        missingClass.Add(plugin);
                    }
                }

                if (missingClass.Count > 0)
                {
                    error = true;
                    OcelotUI.Indent(() =>
                    {
                        OcelotUI.Error("The following plugins are required for this module:");
                        ImGui.SameLine();
                        ImGui.TextUnformatted(string.Join(", ", missingClass));
                    });
                }
            }

            var conflictingPlugin = GetType().GetCustomAttribute<ConflictingPluginAttribute>();
            if (conflictingPlugin != null)
            {
                List<string> conflictingClass = [];
                foreach (var plugin in conflictingPlugin.conflicts)
                {
                    if (DalamudReflector.TryGetDalamudPlugin(plugin, out _, false, true))
                    {
                        conflictingClass.Add(plugin);
                    }
                }

                if (conflictingClass.Count > 0)
                {
                    error = true;
                    OcelotUI.Indent(() =>
                    {
                        OcelotUI.Error("The following plugins are conflicting with this module:");
                        ImGui.SameLine();
                        ImGui.TextUnformatted(string.Join(", ", conflictingClass));
                    });
                }
            }

            if (error)
            {
                return;
            }

            List<string> missingAttrs = [];
            foreach (var handler in GetHandlers())
            {
                if (handler.HasUnloadedRequiredPlugins(out var unloaded))
                {
                    foreach (var item in unloaded)
                    {
                        if (!missingAttrs.Contains(item))
                        {
                            missingAttrs.Add(item);
                        }
                    }
                }
            }

            if (missingAttrs.Count > 0)
            {
                OcelotUI.Indent(() =>
                {
                    OcelotUI.Error("Some options are hidden due to missing plugins: ");
                    ImGui.SameLine();
                    ImGui.TextUnformatted(string.Join(", ", missingAttrs));
                });
            }

            List<string> conflictingAttrs = [];
            foreach (var handler in GetHandlers())
            {
                if (handler.HasLoadedConflictingPlugins(out var loaded))
                {
                    foreach (var item in loaded)
                    {
                        if (!conflictingAttrs.Contains(item))
                        {
                            conflictingAttrs.Add(item);
                        }
                    }
                }
            }

            if (conflictingAttrs.Count > 0)
            {
                OcelotUI.Indent(() =>
                {
                    OcelotUI.Error("Some options are hidden due to conflicting plugins: ");
                    ImGui.SameLine();
                    ImGui.TextUnformatted(string.Join(", ", conflictingAttrs));
                });
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
