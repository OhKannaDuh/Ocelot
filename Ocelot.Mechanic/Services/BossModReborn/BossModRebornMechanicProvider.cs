using Microsoft.Extensions.DependencyInjection;
using Ocelot.Mechanic.Services.BossMod;
using Ocelot.Services.PluginStatus;

namespace Ocelot.Mechanic.Services.BossModReborn;

public class BossModRebornMechanicProvider(IPluginStatus pluginStatus, IServiceProvider services) : IMechanicProvider
{
    public const string Key = "BossModReborn";

    public string InternalName
    {
        get => Key;
    }

    public string DisplayName
    {
        get => "Bossmod Reborn";
    }

    public bool IsAvailable()
    {
        return pluginStatus.IsLoaded(InternalName);
    }

    public IMechanicService Create()
    {
        return services.GetRequiredService<BossModRebornMechanicService>();
    }
}
