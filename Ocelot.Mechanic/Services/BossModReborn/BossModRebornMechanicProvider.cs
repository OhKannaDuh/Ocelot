using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.PluginStatus;

namespace Ocelot.Mechanic.Services.BossMod;

public class BossModRebornMechanicProvider(IPluginStatusService pluginStatus, IServiceProvider services) : IMechanicProvider
{
    public string InternalName
    {
        get => "BossModReborn";
    }

    public string DisplayName
    {
        get => "Bossmod Reborn";
    }

    public int Priority
    {
        get => MechanicPriority.BossModReborn;
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
