using System.Linq;
using Dalamud.Plugin;
using Ocelot.Services.Logger;

namespace Ocelot.Services.PluginStatus;

public class PluginStatus(IDalamudPluginInterface plugin) : IPluginStatus
{
    public bool IsLoaded(string internalName)
    {
        return plugin.InstalledPlugins.FirstOrDefault(p => p.InternalName == internalName)?.IsLoaded ?? false;
    }
}
