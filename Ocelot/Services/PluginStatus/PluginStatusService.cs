using System.Linq;
using Dalamud.Plugin;

namespace Ocelot.Services.PluginStatus;

public class PluginStatusService(IDalamudPluginInterface plugin) : IPluginStatusService
{
    public bool IsLoaded(string internalName)
    {
        return plugin.InstalledPlugins.FirstOrDefault(p => p.InternalName == internalName)?.IsLoaded ?? false;
    }
}
