using Dalamud.Plugin;
using Ocelot.Services.WindowManager;

namespace Ocelot.Windows;

public sealed class ConfigWindow(IDalamudPluginInterface plugin, IConfigRenderer renderer) : OcelotWindow($"{plugin.InternalName} Config"), IConfigWindow
{
    protected override void Render()
    {
        renderer.Render();
    }
}
