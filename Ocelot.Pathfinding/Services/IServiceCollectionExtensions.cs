using Microsoft.Extensions.DependencyInjection;
using Ocelot.Pathfinding.Services.Navmesh;
using Ocelot.Services.Pathfinding;

namespace Ocelot.Pathfinding.Services;

public static class IServiceCollectionExtensions
{
    public static void LoadPathfinding(this IServiceCollection services)
    {
        services.AddTransient<NavmeshPathfinder>();
        services.AddSingleton<IPathfindingProvider, NavmeshPathfindingProvider>();

        services.AddSingleton<IPathfinder, DynamicPathfinder>();
        services.AddSingleton<IPathfindingPriorityService, PathfindingPriorityService>();
    }
}
