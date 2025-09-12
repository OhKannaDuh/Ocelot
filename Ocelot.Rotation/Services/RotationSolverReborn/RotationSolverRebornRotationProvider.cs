using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.PluginStatus;

namespace Ocelot.Rotation.Services.RotationSolverReborn;

public class RotationSolverRebornRotationProvider(IPluginStatusService pluginStatus, IServiceProvider services) : IRotationProvider
{
    public string InternalName {
        get => "RotationSolver";
    }

    public string DisplayName {
        get => "Rotation Solver Reborn";
    }

    public int Priority {
        get => RotationPriority.RotationSolverReborn;
    }

    public bool IsAvailable()
    {
        return pluginStatus.IsLoaded(InternalName);
    }

    public IRotationService Create()
    {
        return services.GetRequiredService<RotationSolverRebornRotationService>();
    }
}
