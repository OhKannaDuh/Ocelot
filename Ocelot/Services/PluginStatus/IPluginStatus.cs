namespace Ocelot.Services.PluginStatus;

public interface IPluginStatus
{
    bool IsLoaded(string internalName);
}
