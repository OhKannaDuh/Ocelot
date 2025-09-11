using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Ocelot.Intents;
using Ocelot.Services;

namespace Ocelot.Windows;

public abstract class OcelotMainWindow(OcelotPlugin plugin, OcelotConfig config) : OcelotWindow(plugin, config), IInitializable
{
    private IReadOnlyList<IMainRenderable> renderables = [];

    public void PreInitialize() { }

    public void Initialize() { }

    public void PostInitialize()
    {
        if (!WindowManager.TryGetWindow<OcelotConfigWindow>(out _))
        {
            return;
        }

        TitleBarButtons.Add(new TitleBarButton {
            Click = (m) => {
                if (m != ImGuiMouseButton.Left)
                {
                    return;
                }

                WindowManager.ToggleConfigUI();
            },
            Icon = FontAwesomeIcon.Cog,
            IconOffset = new Vector2(2, 2),
            ShowTooltip = () => ImGui.SetTooltip(Translator.T("windows.main.titlebar_buttons.toggle_config")),
        });

        renderables = OcelotServices.Container.GetAll<IMainRenderable>().ToList();
    }

    protected override string GetWindowName()
    {
        return $"{Translator.T("windows.main.title")}##Main";
    }

    protected override void Render(RenderContext context)
    {
        foreach (var r in renderables)
        {
            if (r is IToggleable { IsEnabled: false })
            {
                continue;
            }

            r.RenderMainUi(context);
        }
    }
}
