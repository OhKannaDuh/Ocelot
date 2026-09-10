using Dalamud.Plugin;

namespace Ocelot.Services.PluginStatus;

public class PluginStatus(IDalamudPluginInterface plugin) : IPluginStatus
{
    public bool IsLoaded(string internalName)
    {
        // InstalledPlugins can list unloaded copies of the same InternalName; require IsLoaded.
        return plugin.InstalledPlugins.Any(p => p.InternalName == internalName && p.IsLoaded);
    }
}
