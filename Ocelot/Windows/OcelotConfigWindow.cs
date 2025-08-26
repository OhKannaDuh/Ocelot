using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Ocelot.Modules;
using Ocelot.UI;

namespace Ocelot.Windows;

public abstract class OcelotConfigWindow(OcelotPlugin plugin, OcelotConfig pluginConfig) : OcelotWindow(plugin, pluginConfig)
{
    private IModule? selectedConfigModule;

    public override void PostInitialize()
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(384, 0),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };
    }

    protected override void Render(RenderContext context)
    {
        var groups = ModuleConfigGroupRegistry.GetConfigGroupOrder().ToList();
        if (!groups.Any())
        {
            return;
        }

        var core = Plugin.Modules.GetModule<OcelotCoreModule>();
        if (selectedConfigModule is null)
        {
            var savedId = core.Config.SelectedConfigModuleId;

            if (!string.IsNullOrWhiteSpace(savedId))
            {
                selectedConfigModule = Plugin.Modules.All().FirstOrDefault(m => m?.GetType().FullName == savedId, null);
            }

            foreach (var key in groups)
            {
                var modules = Plugin.Modules.GetModulesByGroup(key).ToList();
                if (!modules.Any())
                {
                    continue;
                }

                selectedConfigModule ??= modules.FirstOrDefault();
            }
        }


        using (ImRaii.Child("##LeftPanel", new Vector2(300, 0), true))
        {
            foreach (var key in groups)
            {
                var modules = Plugin.Modules.GetModulesByGroup(key).Where(m => m.Config != null).ToList();
                if (!modules.Any())
                {
                    continue;
                }

                var group = ModuleConfigGroupRegistry.Get(key);
                OcelotUI.Title(I18N.T(group.TranslationKey));

                OcelotUI.Indent(() =>
                {
                    foreach (var module in modules)
                    {
                        if (module.Config == null)
                        {
                            continue;
                        }

                        var name = module.Config.GetType().Name;
                        var title = module.Config.GetTitle();
                        if (title != null)
                        {
                            name = title;
                        }

                        var selected = module == selectedConfigModule;
                        if (ImGui.Selectable(name, selected))
                        {
                            selectedConfigModule = module;
                            core.Config.SelectedConfigModuleId = selectedConfigModule.GetType().FullName;
                            pluginConfig.Save();
                        }
                    }
                });
            }
        }

        ImGui.SameLine();

        using (ImRaii.Child("##RightPanel", new Vector2(0, 0), true))
        {
            selectedConfigModule?.RenderConfigUi(context);
        }
    }

    protected override string GetWindowName()
    {
        return $"{I18N.T("windows.config.title")}##Config";
    }
}
