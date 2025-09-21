using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.Pathfinding;
using Ocelot.Services.PluginStatus;

namespace Ocelot.Pathfinding.Services.Navmesh;

public class NavmeshPathfindingProvider(IPluginStatus pluginStatus, IServiceProvider services) : IPathfindingProvider
{
    public const string Key = "vnavmesh";

    public string InternalName
    {
        get => Key;
    }

    public string DisplayName
    {
        get => Key;
    }

    public bool IsAvailable()
    {
        return pluginStatus.IsLoaded(InternalName);
    }

    public IPathfinder Create()
    {
        return services.GetRequiredService<NavmeshPathfinder>();
    }
}
