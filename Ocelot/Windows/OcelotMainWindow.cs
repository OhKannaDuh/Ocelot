using Dalamud.Interface;
using ImGuiNET;

namespace Ocelot.Windows;

public abstract class OcelotMainWindow(OcelotPlugin plugin, IOcelotConfig config) : OcelotWindow(plugin, config)
{
    public override void PostInitialize()
    {
        if (!plugin.windows.TryGetWindow<OcelotConfigWindow>(out var _))
        {
            return;
        }

        TitleBarButtons.Add(new()
        {
            Click = (m) =>
            {
                if (m != ImGuiMouseButton.Left)
                {
                    return;
                }

                plugin.windows.ToggleConfigUI();
            },
            Icon = FontAwesomeIcon.Cog,
            IconOffset = new(2, 2),
            ShowTooltip = () => ImGui.SetTooltip(I18N.T("windows.main.buttons.toggle_config")),
        });
    }

    protected override string GetWindowName()
    {
        return $"{I18N.T("windows.main.title")}##Main";
    }
}
