using Microsoft.Extensions.DependencyInjection;
using Ocelot.Mechanic.Services.BossMod;
using Ocelot.Mechanic.Services.BossModReborn;

namespace Ocelot.Mechanic.Services;

public static class IServiceCollectionExtensions
{
    public static void LoadMechanics(this IServiceCollection services)
    {
        services.AddTransient<BossModMechanicService>();
        services.AddSingleton<IMechanicProvider, BossModMechanicProvider>();

        services.AddTransient<BossModRebornMechanicService>();
        services.AddSingleton<IMechanicProvider, BossModRebornMechanicProvider>();

        services.AddSingleton<IMechanicService, DynamicMechanicService>();
        services.AddSingleton<IMechanicPriorityService, MechanicPriorityService>();
    }
}
