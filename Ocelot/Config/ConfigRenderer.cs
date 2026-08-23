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

namespace Ocelot.Config;

public class ConfigRenderer : IConfigRenderer
{
    private readonly ITranslator translator;

    private readonly IServiceProvider services;

    private readonly IConfigSaver saver;

    private readonly IAutoConfig[] configs;

    private IAutoConfig? current;

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

    public void Render()
    {
        // Font-relative, not a hardcoded 300px: at larger UI scales a fixed sidebar clips its own
        // labels, and at small scales it wastes half the window.
        var sidebarWidth = Math.Clamp(ImGui.GetFontSize() * 15f, 220f, 380f);

        using (ImRaii.Child("##LeftPanel", new Vector2(sidebarWidth, 0), true))
        {
            foreach (var uConfig in ungrouped)
            {
                var selected = current == uConfig;
                if (ImGui.Selectable(uConfig.Label(translator), selected))
                {
                    current = uConfig;
                }
            }

            foreach (var (key, gConfigs) in grouped.OrderBy(kvp =>
                         kvp.Value
                             .Select(c => c.GetType().GetCustomAttribute<ConfigGroupAttribute>()?.GroupOrder ?? 0)
                             .DefaultIfEmpty(0)
                             .Min())
                     .ThenBy(kvp => kvp.Key))
            {
                // Single-page groups: show one top-level entry (avoid "Mob Farmer → Mob Farmer").
                if (gConfigs.Count == 1)
                {
                    var only = gConfigs[0];
                    var selected = current == only;
                    if (ImGui.Selectable(only.Label(translator), selected))
                    {
                        current = only;
                    }

                    continue;
                }

                // Muted, from the active theme rather than a fixed colour, so a group header reads
                // as a category and not as another clickable row at the same weight as its pages.
                ImGui.Spacing();
                ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(ImGuiCol.TextDisabled));
                ImGui.TextUnformatted(translator.T($"config_group.{key}.label"));
                ImGui.PopStyleColor();

                ImGui.Indent(16);
                foreach (var gConfig in gConfigs)
                {
                    var selected = current == gConfig;
                    if (ImGui.Selectable(gConfig.Label(translator), selected))
                    {
                        current = gConfig;
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
            var type = current.GetType();

            // Page blurb when present (mirrors main-window mode descriptions).
            var descKey = current.GetTooltipKey();
            if (translator.Has(descKey))
            {
                ImGui.TextWrapped(translator.T(descKey));
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();
            }

            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(prop => (prop, attr: prop.GetCustomAttributes().OfType<UIFieldAttribute>().SingleOrDefault()))
                .Where(x => x.attr is not null)
                .OrderBy(x => x.attr!.Order)
                .ThenBy(x => x.prop.Name, StringComparer.Ordinal)
                .ToList();

            var configSnake = type.Name.Replace("Config", "").ToSnakeCase();
            string? lastSection = null;

            foreach (var (prop, attr) in props)
            {
                ImGui.PushID(prop.Name);

                var fieldAttr = attr!;
                if (!string.IsNullOrEmpty(fieldAttr.Section) && fieldAttr.Section != lastSection)
                {
                    // Separate every section the same way, including the first. Previously the first
                    // header butted straight against the page blurb while the rest were spaced, so
                    // the top of every page looked subtly different from the body.
                    ImGui.Spacing();
                    if (lastSection != null)
                    {
                        ImGui.Separator();
                        ImGui.Spacing();
                    }

                    var sectionKey = $"config.{configSnake}.sections.{fieldAttr.Section}";

                    // Accent colour from the theme — section headers sat at the same weight as the
                    // field labels beneath them, which is what made long pages hard to scan.
                    // (ImGui.SeparatorText would be the idiomatic choice but is not in these bindings.)
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(ImGuiCol.CheckMark));
                    ImGui.TextUnformatted(translator.Has(sectionKey) ? translator.T(sectionKey) : fieldAttr.Section);
                    ImGui.PopStyleColor();

                    ImGui.Spacing();
                    lastSection = fieldAttr.Section;
                }

                var indentPx = fieldAttr.Indent > 0 ? fieldAttr.Indent * 16f : 0f;
                if (indentPx > 0f)
                {
                    ImGui.Indent(indentPx);
                }

                var requiresDisabled = false;
                if (!string.IsNullOrEmpty(fieldAttr.Requires))
                {
                    var required = type.GetProperty(fieldAttr.Requires, BindingFlags.Instance | BindingFlags.Public);
                    if (required?.PropertyType == typeof(bool) && required.GetValue(current) is false)
                    {
                        requiresDisabled = true;
                    }
                }

                if (!requiresDisabled && !string.IsNullOrEmpty(fieldAttr.DisabledWhen))
                {
                    var blocker = type.GetProperty(fieldAttr.DisabledWhen, BindingFlags.Instance | BindingFlags.Public);
                    if (blocker?.PropertyType == typeof(bool) && blocker.GetValue(current) is true)
                    {
                        requiresDisabled = true;
                    }
                }

                bool changed;
                if (requiresDisabled)
                {
                    using (ImRaii.Disabled())
                    {
                        var renderer = GetRenderer(fieldAttr);
                        changed = InvokeRenderer(renderer, current, prop, fieldAttr, type);
                    }
                }
                else
                {
                    var renderer = GetRenderer(fieldAttr);
                    changed = InvokeRenderer(renderer, current, prop, fieldAttr, type);
                }

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
        }

        if (dirty)
        {
            saver.Save();
        }
    }
}
