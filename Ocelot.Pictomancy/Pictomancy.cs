using Dalamud.Plugin;
using Ocelot.Lifecycle;
using Pictomancy;

namespace Ocelot.Pictomancy;

public class Pictomancy(IDalamudPluginInterface plugin) : IOnStart, IOnStop
{
    public void OnStart()
    {
        PictoService.Initialize(plugin);
    }

    public void OnStop()
    {
        PictoService.Dispose();
    }
}
