using System.Numerics;
using Ocelot.Lifecycle;
using Ocelot.Services.Logger;
using Ocelot.Services.Pathfinding;
using Path = Ocelot.Services.Pathfinding.Path;

namespace Ocelot.Pathfinding.Services;

public class DynamicPathfinder(
    IEnumerable<IPathfindingProvider> providers,
    IPathfindingPriorityService priority,
    ILogger logger
) : IPathfinder, IOnPreUpdate
{
    private string currentInternalName = "";

    private IPathfinder? current;

    public void PreUpdate()
    {
        var order = priority.GetPriority().ToList();
        var rank = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < order.Count; i++)
        {
            rank[order[i]] = i;
        }

        var bestMatch = providers
            .Where(p => p.IsAvailable())
            .OrderBy(p => rank.GetValueOrDefault(p.InternalName, int.MaxValue))
            .ThenBy(p => p.InternalName, StringComparer.Ordinal)
            .FirstOrDefault();

        if ((bestMatch?.InternalName ?? "") == currentInternalName)
        {
            return;
        }

        currentInternalName = bestMatch?.InternalName ?? "";
        current = bestMatch?.Create();

        logger.Info($"[DynamicPathfindingService] Dynamic pathfinding service has been updated to {currentInternalName}");
    }

    public PathfindingState GetState()
    {
        return current?.GetState() ?? PathfindingState.Idle;
    }


    public void PathfindAndMoveTo(PathfinderConfig config)
    {
        current?.PathfindAndMoveTo(config);
    }

    public Task<Path> Pathfind(PathfinderConfig config)
    {
        return current == null ? Task.FromResult(Path.Blank(this)) : current.Pathfind(config);
    }

    public void FollowPath(Path path)
    {
        current?.FollowPath(path);
    }

    public Vector3 SnapToMesh(Vector3 point, float extent)
    {
        return current?.SnapToMesh(point, extent) ?? point;
    }

    public void Stop()
    {
        current?.Stop();
    }
}
