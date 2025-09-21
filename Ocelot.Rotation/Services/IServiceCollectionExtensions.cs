using Microsoft.Extensions.DependencyInjection;
using Ocelot.Rotation.Services.BossMod;
using Ocelot.Rotation.Services.RotationSolverReborn;
using Ocelot.Rotation.Services.Wrath;

namespace Ocelot.Rotation.Services;

public static class IServiceCollectionExtensions
{
    public static void LoadRotations(this IServiceCollection services)
    {
        services.AddTransient<WrathRotationService>();
        services.AddSingleton<IRotationProvider, WrathRotationProvider>();

        services.AddTransient<BossModRotationService>();
        services.AddSingleton<IRotationProvider, BossModRotationProvider>();

        services.AddTransient<RotationSolverRebornRotationService>();
        services.AddSingleton<IRotationProvider, RotationSolverRebornRotationProvider>();

        services.AddSingleton<IRotationService, DynamicRotationService>();
        services.AddSingleton<IRotationPriorityService, RotationPriorityService>();
    }
}
