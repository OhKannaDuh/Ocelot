using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.PluginStatus;

namespace Ocelot.Rotation.Services.RotationSolverReborn;

public class RotationSolverRebornRotationProvider(IPluginStatus pluginStatus, IServiceProvider services) : IRotationProvider
{
    public const string Key = "RotationSolver";
    
    public string InternalName
    {
        get => Key;
    }

    public string DisplayName
    {
        get => "Rotation Solver Reborn";
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
