using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface;
using ECommons.Reflection;
using ImGuiNET;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace Ocelot.Config.Handlers;

public class RenderContext
{
    private readonly PropertyInfo prop;

    private readonly Type type;

    private readonly ModuleConfig self;

    public RenderContext(PropertyInfo prop, Type type, ModuleConfig self)
    {
        this.prop = prop;
        this.type = type;
        this.self = self;
    }


    public bool IsValid() => prop.PropertyType == type;

    public bool ShouldRender()
    {
        if (HasUnloadedRequiredPlugins(out var _))
        {
            return false;
        }

        if (HasLoadedConflictingPlugins(out var _))
        {
            return false;
        }

        var render = prop.GetCustomAttribute<DependsOnAttribute>();
        if (render == null)
        {
            return true;
        }

        foreach (var dependency in render.dependencies)
        {
            var dependent = self.GetType().GetProperty(dependency);
            if (dependent == null || dependent.PropertyType != typeof(bool))
            {
                return false;
            }

            if (!(bool)(dependent.GetValue(self) ?? false))
            {
                return false;
            }
        }

        return true;
    }

    public bool HasUnloadedRequiredPlugins(out List<string> unloaded)
    {
        unloaded = [];

        var attr = prop.GetCustomAttribute<RequiredPluginAttribute>();
        if (attr == null)
            return false;

        foreach (var plugin in attr.dependencies)
        {
            if (!DalamudReflector.TryGetDalamudPlugin(plugin, out _, false, true))
            {
                unloaded.Add(plugin);
            }
        }

        return unloaded.Count > 0;
    }



    public bool HasLoadedConflictingPlugins(out List<string> loaded)
    {
        loaded = [];

        var attr = prop.GetCustomAttribute<ConflictingPluginAttribute>();
        if (attr == null)
            return false;

        foreach (var plugin in attr.conflicts)
        {
            if (DalamudReflector.TryGetDalamudPlugin(plugin, out _, false, true))
            {
                loaded.Add(plugin);
            }
        }

        return loaded.Count > 0;
    }


    public bool IsExperimental() => prop.GetCustomAttribute<ExperimentalAttribute>() != null;

    public uint GetIndentation() => prop.GetCustomAttribute<IndentAttribute>()?.depth ?? 0;

    public void Experimental()
    {
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.9f, 0.0f, 1.0f));
        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.TextUnformatted(FontAwesomeIcon.ExclamationTriangle.ToIconString());
        ImGui.PopFont();
        ImGui.PopStyleColor();

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Experimental");
        }

        ImGui.SameLine();
    }

    public bool IsIllegal() => prop.GetCustomAttribute<IllegalAttribute>() != null;

    public void Illegal()
    {
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.2f, 0.2f, 1.0f));
        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.TextUnformatted(FontAwesomeIcon.ExclamationTriangle.ToIconString());
        ImGui.PopFont();
        ImGui.PopStyleColor();

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("May lead to a ban. Use at your own risk.");
        }

        ImGui.SameLine();
    }

    public void Tooltip()
    {
        var tooltip = prop.GetCustomAttribute<TooltipAttribute>();
        if (tooltip != null && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(I18N.T(tooltip.translation_key));
        }
    }

    public string GetLabel()
    {
        var key = prop.GetCustomAttribute<LabelAttribute>()?.translation_key;
        if (key == null)
        {
            return prop.Name;
        }

        return I18N.T(key);
    }

    public string GetLabelWithId() => $"{GetLabel()}##{prop.GetHashCode()}";

    public object? GetValue() => prop.GetValue(self);

    public void SetValue(object? value) => prop.SetValue(self, value);
}
