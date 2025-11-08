using System.Numerics;
using System.Reflection;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Config.Fields;
using Ocelot.Config.Renderers;
using Ocelot.Graphics;
using Ocelot.Services.Translation;
using Ocelot.Services.WindowManager;

namespace Ocelot.Config;

public class ConfigRenderer : IConfigRenderer
{
    private readonly ITranslator translator;

    private readonly Dictionary<Type, object> renderers = [];

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

        current = configs.FirstOrDefault();
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
        using (ImRaii.Child("##LeftPanel", new Vector2(300, 0), true))
        {
            foreach (var uConfig in ungrouped)
            {
                var selected = current == uConfig;
                if (ImGui.Selectable(uConfig.Label(translator), selected))
                {
                    current = uConfig;
                }

                uConfig.Tooltip(translator);
            }

            foreach (var (key, gConfigs) in grouped)
            {
                ImGui.Text(translator.T($"config_group.{key}.label"));
                ImGui.Indent(16);
                foreach (var gConfig in gConfigs)
                {
                    var selected = current == gConfig;
                    if (ImGui.Selectable(gConfig.Label(translator), selected))
                    {
                        current = gConfig;
                    }

                    gConfig.Tooltip(translator);
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

            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var attr = prop.GetCustomAttributes().OfType<UIFieldAttribute>().SingleOrDefault();
                if (attr is null)
                {
                    continue;
                }

                ImGui.PushID(prop.Name);

                var renderer = GetRenderer(attr);
                var changed = InvokeRenderer(renderer, current, prop, attr, type);

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
