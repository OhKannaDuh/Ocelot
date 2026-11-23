using System.Numerics;
using System.Reflection;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Config.Fields;
using Ocelot.Config.Renderers;
using Ocelot.Extensions;
using Ocelot.Graphics;
using Ocelot.Services.Translation;
using Ocelot.Services.WindowManager;
using Ocelot.UI;

namespace Ocelot.Config;

public class ConfigRenderer : IConfigRenderer
{
    private readonly ITranslator translator;

    private readonly IServiceProvider services;

    private readonly IConfigSaver saver;

    private readonly IAutoConfig[] configs;

    private IAutoConfig? current;

    private string search = string.Empty;

    private readonly List<IAutoConfig> ungrouped = [];

    private readonly Dictionary<string, List<IAutoConfig>> grouped = [];

    public ConfigRenderer(IEnumerable<IAutoConfig> registered, IServiceProvider services, IConfigSaver saver, ITranslator translator)
    {
        configs = registered.ToArray();
        this.services = services;
        this.saver = saver;
        this.translator = translator;

        foreach (var config in configs)
        {
            if (config is null || config.GetType().GetCustomAttribute<ConfigHiddenAttribute>() != null)
            {
                continue;
            }

            var attr = config.GetType().GetCustomAttribute<ConfigGroupAttribute>();
            if (attr == null)
            {
                ungrouped.Add(config);
                continue;
            }

            var key = attr.Key;
            if (!grouped.ContainsKey(key))
            {
                grouped.Add(key, []);
            }

            grouped[key].Add(config);
        }

        foreach (var kvp in grouped)
        {
            kvp.Value.Sort((a, b) =>
            {
                var aOrder = a.GetType().GetCustomAttribute<ConfigGroupAttribute>()?.Order ?? 0;
                var bOrder = b.GetType().GetCustomAttribute<ConfigGroupAttribute>()?.Order ?? 0;
                var cmp = aOrder.CompareTo(bOrder);

                return cmp != 0 ? cmp : string.Compare(a.GetType().Name, b.GetType().Name, StringComparison.Ordinal);
            });
        }

        current = configs.FirstOrDefault(c =>
            c is not null && c.GetType().GetCustomAttribute<ConfigHiddenAttribute>() == null);
    }

    private bool pendingSave;

    private object GetRenderer(UIFieldAttribute attr)
    {
        var serviceType = typeof(IFieldRenderer<>).MakeGenericType(attr.GetType());
        var service = services.GetService(serviceType);
        if (service is not null)
        {
            return service;
        }

        return ActivatorUtilities.CreateInstance(services, attr.RendererType);
    }

    private bool InvokeRenderer(object renderer, object target, PropertyInfo prop, UIFieldAttribute attr, Type owner)
    {
        var type = renderer.GetType().GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFieldRenderer<>));

        var method = type.GetMethod("Render")!;
        return (bool)method.Invoke(renderer, [target, prop, attr, owner, translator])!;
    }

    public void SelectGroup(string groupKey)
    {
        if (grouped.TryGetValue(groupKey, out var gConfigs) && gConfigs.Count > 0)
        {
            current = gConfigs[0];
            search = string.Empty;
            return;
        }

        var page = configs.FirstOrDefault(c => c?.GetType().Name.Replace("Config", "").ToSnakeCase() == groupKey);
        if (page != null)
        {
            current = page;
            search = string.Empty;
        }
    }

    public bool RenderField(IAutoConfig config, string propertyName)
    {
        var type = config.GetType();
        var prop = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        var attr = prop?.GetCustomAttributes().OfType<UIFieldAttribute>().SingleOrDefault();
        if (prop == null || attr == null)
        {
            return false;
        }

        ImGui.PushID($"{type.Name}.{propertyName}");
        var changed = DrawField(config, type, prop, attr);
        ImGui.PopID();

        pendingSave |= changed;
        if (pendingSave && !ImGui.IsAnyItemActive())
        {
            pendingSave = false;
            saver.Save();
        }

        return changed;
    }

    private bool DrawField(IAutoConfig config, Type type, PropertyInfo prop, UIFieldAttribute fieldAttr)
    {
        var requiresDisabled = false;
        if (!string.IsNullOrEmpty(fieldAttr.Requires))
        {
            var required = type.GetProperty(fieldAttr.Requires, BindingFlags.Instance | BindingFlags.Public);
            if (required?.PropertyType == typeof(bool) && required.GetValue(config) is false)
            {
                requiresDisabled = true;
            }
        }

        if (!requiresDisabled && !string.IsNullOrEmpty(fieldAttr.DisabledWhen))
        {
            var blocker = type.GetProperty(fieldAttr.DisabledWhen, BindingFlags.Instance | BindingFlags.Public);
            if (blocker?.PropertyType == typeof(bool) && blocker.GetValue(config) is true)
            {
                requiresDisabled = true;
            }
        }

        using (ImRaii.Disabled(requiresDisabled))
        {
            return InvokeRenderer(GetRenderer(fieldAttr), config, prop, fieldAttr, type);
        }
    }

    public void Render()
    {
        // Font-relative width — a fixed 300px clips labels at large UI scale.
        var sidebarWidth = Math.Clamp(ImGui.GetFontSize() * 15f, 220f, 380f);

        using (ImRaii.Child("##LeftPanel", new Vector2(sidebarWidth, 0), true))
        {
            ImGui.SetNextItemWidth(-1);
            ImGui.InputTextWithHint(
                "##config_search",
                translator.Has("generic.search_settings") ? translator.T("generic.search_settings") : "Search settings...",
                ref search,
                64);
            ImGui.Spacing();

            foreach (var uConfig in ungrouped)
            {
                var selected = current == uConfig;
                if (ImGui.Selectable(uConfig.Label(translator), selected))
                {
                    current = uConfig;
                    search = string.Empty;
                }
            }

            foreach (var (key, gConfigs) in grouped.OrderBy(kvp =>
                         kvp.Value
                             .Select(c => c.GetType().GetCustomAttribute<ConfigGroupAttribute>()?.GroupOrder ?? 0)
                             .DefaultIfEmpty(0)
                             .Min())
                     .ThenBy(kvp => kvp.Key))
            {
                if (gConfigs.Count == 1)
                {
                    var only = gConfigs[0];
                    var selected = current == only;
                    if (ImGui.Selectable(only.Label(translator), selected))
                    {
                        current = only;
                        search = string.Empty;
                    }

                    continue;
                }

                ImGui.Spacing();
                ImGui.TextColored(OcelotUi.Header, translator.T($"config_group.{key}.label"));

                ImGui.Indent(16);
                foreach (var gConfig in gConfigs)
                {
                    var selected = current == gConfig;
                    if (ImGui.Selectable(gConfig.Label(translator), selected))
                    {
                        current = gConfig;
                        search = string.Empty;
                    }
                }

                ImGui.Unindent(16);
            }
        }

        ImGui.SameLine();

        if (current == null)
        {
            ImGui.TextColored(Color.Red.ToRgba(), "Unable to render config");
            return;
        }

        var dirty = false;
        using (ImRaii.Child("##RightPanel", new Vector2(0, 0), true))
        {
            var filter = search.Trim();
            if (filter.Length == 0)
            {
                dirty = RenderFields(current, null);
            }
            else
            {
                foreach (var config in configs)
                {
                    if (config is null || config.GetType().GetCustomAttribute<ConfigHiddenAttribute>() != null)
                    {
                        continue;
                    }

                    ImGui.PushID(config.GetType().Name);
                    dirty |= RenderFields(config, filter);
                    ImGui.PopID();
                }
            }
        }

        // Save once the slider or text box is let go, not on every frame it changes.
        pendingSave |= dirty;
        if (pendingSave && !ImGui.IsAnyItemActive())
        {
            pendingSave = false;
            saver.Save();
        }
    }

    private bool RenderFields(IAutoConfig config, string? filter)
    {
        var dirty = false;
        {
            var current = config;
            var type = current.GetType();

            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(prop => (prop, attr: prop.GetCustomAttributes().OfType<UIFieldAttribute>().SingleOrDefault()))
                .Where(x => x.attr is not null)
                .Where(x => filter == null || x.prop.Label(type, translator).Contains(filter, StringComparison.CurrentCultureIgnoreCase))
                .OrderBy(x => x.attr!.Order)
                .ThenBy(x => x.prop.Name, StringComparer.Ordinal)
                .ToList();

            if (filter != null)
            {
                if (props.Count == 0)
                {
                    return false;
                }

                ImGui.TextColored(OcelotTheme.GoldBright, config.Label(translator));
                ImGui.Separator();
            }
            else
            {
                var descKey = current.GetTooltipKey();
                if (translator.Has(descKey))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, OcelotUi.Muted);
                    ImGui.TextWrapped(translator.T(descKey));
                    ImGui.PopStyleColor();
                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.Spacing();
                }
            }

            var configSnake = type.Name.Replace("Config", "").ToSnakeCase();
            string? lastSection = null;

            foreach (var (prop, attr) in props)
            {
                ImGui.PushID(prop.Name);

                var fieldAttr = attr!;
                if (!string.IsNullOrEmpty(fieldAttr.Section) && fieldAttr.Section != lastSection)
                {
                    ImGui.Spacing();
                    if (lastSection != null)
                    {
                        ImGui.Separator();
                        ImGui.Spacing();
                    }

                    var sectionKey = $"config.{configSnake}.sections.{fieldAttr.Section}";

                    ImGui.TextColored(OcelotUi.Header, translator.Has(sectionKey) ? translator.T(sectionKey) : fieldAttr.Section);

                    ImGui.Spacing();
                    lastSection = fieldAttr.Section;
                }

                var indentPx = fieldAttr.Indent > 0 ? fieldAttr.Indent * 16f : 0f;
                if (indentPx > 0f)
                {
                    ImGui.Indent(indentPx);
                }

                var changed = DrawField(current, type, prop, fieldAttr);

                if (indentPx > 0f)
                {
                    ImGui.Unindent(indentPx);
                }

                if (changed)
                {
                    dirty = true;
                }

                ImGui.PopID();
            }

            if (filter != null)
            {
                ImGui.Spacing();
            }
        }

        return dirty;
    }
}
