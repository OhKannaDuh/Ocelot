namespace Ocelot.Services.PluginStatus;

public interface IPluginStatusService
{
    bool IsLoaded(string internalName);
}
