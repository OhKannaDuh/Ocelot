using Dalamud.Plugin;
using Ocelot.Lifecycle;
using Pictomancy;

namespace Ocelot.Pictomancy;

public class Pictomancy(IDalamudPluginInterface plugin) : IOnStart, IOnStop
{
    public void OnStart()
    {
        PctService.Initialize(plugin);
    }

    public void OnStop()
    {
        PctService.Dispose();
    }
}
