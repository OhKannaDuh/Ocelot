using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.PluginStatus;

namespace Ocelot.Mechanic.Services.BossMod;

public class BossModMechanicProvider(IPluginStatus pluginStatus, IServiceProvider services) : IMechanicProvider
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

    public IMechanicService Create()
    {
        return services.GetRequiredService<BossModMechanicService>();
    }
}
