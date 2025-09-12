using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.PluginStatus;

namespace Ocelot.Mechanic.Services.BossMod;

public class BossModMechanicProvider(IPluginStatusService pluginStatus, IServiceProvider services) : IMechanicProvider
{
    public string InternalName {
        get => "BossMod";
    }

    public string DisplayName {
        get => "Boss Mod";
    }

    public int Priority {
        get => MechanicPriority.BossMod;
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
