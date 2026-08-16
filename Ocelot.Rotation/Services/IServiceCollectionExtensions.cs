using Microsoft.Extensions.DependencyInjection;
using Ocelot.Rotation.Services.BossMod;
using Ocelot.Rotation.Services.RotationSolverReborn;
using Ocelot.Rotation.Services.Wrath;

namespace Ocelot.Rotation.Services;

public static class IServiceCollectionExtensions
{
    public static void LoadRotations(this IServiceCollection services)
    {
        services.AddSingleton<CombatAiPresetNaming>();
        services.AddSingleton<BossModPresetEngine>();
        services.AddSingleton<BossModMiscAiBackend>();

        services.AddSingleton<ICombatAiBackend>(sp => sp.GetRequiredService<BossModMiscAiBackend>());

        services.AddSingleton<IJobRotationBackend, WrathJobRotation>();
        services.AddSingleton<IJobRotationBackend, RsrJobRotation>();
        services.AddSingleton<IJobRotationBackend>(sp =>
            new BossModFullArJobRotation(sp.GetRequiredService<BossModPresetEngine>(), JobRotationBackendKind.BossMod));
        services.AddSingleton<IJobRotationBackend>(sp =>
            new BossModFullArJobRotation(sp.GetRequiredService<BossModPresetEngine>(), JobRotationBackendKind.BossModReborn));

        services.AddSingleton<ICombatRotationSession, CombatRotationSession>();
    }
}
