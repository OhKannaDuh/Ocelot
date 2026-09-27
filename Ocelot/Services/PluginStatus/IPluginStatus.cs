namespace Ocelot.Services.PluginStatus;

public interface IPluginStatus
{
    bool IsLoaded(string internalName);

    /// <summary>True when Dalamud lists the plugin as installed (enabled or not).</summary>
    bool IsInstalled(string internalName);
}
