using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.PluginStatus;

namespace Ocelot.Rotation.Services.BossMod;

public class BossModRotationProvider(IPluginStatus pluginStatus, IServiceProvider services) : IRotationProvider
{
    public const string Key = "BossMod";
    
    public string InternalName
    {
        get => Key;
    }

    public string DisplayName
    {
        get => "Boss Mod";
    }

    public bool IsAvailable()
    {
        return pluginStatus.IsLoaded(InternalName);
    }

    public IRotationService Create()
    {
        return services.GetRequiredService<BossModRotationService>();
    }
}
