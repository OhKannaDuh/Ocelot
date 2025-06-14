using Dalamud.Interface;
using ImGuiNET;

namespace Ocelot.Windows;

public abstract class OcelotMainWindow : OcelotWindow
{
    public OcelotMainWindow(OcelotPlugin plugin, IOcelotConfig config)
        : base(plugin, config, $"{I18N.T("windows.main.title")}##Main") { }

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
            ShowTooltip = () => ImGui.SetTooltip("Toggle config window"),
        });
    }
}
