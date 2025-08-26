using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;

namespace Ocelot.Windows;

public abstract class OcelotMainWindow(OcelotPlugin plugin, OcelotConfig pluginConfig) : OcelotWindow(plugin, pluginConfig)
{
    public override void PostInitialize()
    {
        if (!Plugin.Windows.TryGetWindow<OcelotConfigWindow>(out _))
        {
            return;
        }

        TitleBarButtons.Add(new TitleBarButton
        {
            Click = (m) =>
            {
                if (m != ImGuiMouseButton.Left)
                {
                    return;
                }

                Plugin.Windows.ToggleConfigUI();
            },
            Icon = FontAwesomeIcon.Cog,
            IconOffset = new Vector2(2, 2),
            ShowTooltip = () => ImGui.SetTooltip(I18N.T("windows.main.titlebar_buttons.toggle_config")),
        });
    }

    protected override string GetWindowName()
    {
        return $"{I18N.T("windows.main.title")}##Main";
    }
}
