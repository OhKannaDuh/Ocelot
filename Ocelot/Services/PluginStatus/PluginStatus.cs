using Dalamud.Plugin;

namespace Ocelot.Services.PluginStatus;

public class PluginStatus(IDalamudPluginInterface plugin) : IPluginStatus
{
    public bool IsLoaded(string internalName)
    {
        // Dalamud can list more than one install with the same InternalName (old Rotation Solver +
        // Reborn, testing vs release, a disabled profile copy). FirstOrDefault would pick the
        // unloaded one and claim the plugin is missing while it is actually running.
        return plugin.InstalledPlugins.Any(p => p.InternalName == internalName && p.IsLoaded);
    }
}
