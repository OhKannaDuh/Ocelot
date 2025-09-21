using Dalamud.Plugin;
using Ocelot.Services.WindowManager;

namespace Ocelot.Windows;

public sealed class MainWindow(IDalamudPluginInterface plugin, IMainRenderer renderer) : OcelotWindow(plugin.InternalName), IMainWindow
{
    protected override void Render()
    {
        renderer.Render();
    }
}
