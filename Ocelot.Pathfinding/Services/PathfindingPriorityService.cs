using Ocelot.Pathfinding.Services.Navmesh;

namespace Ocelot.Pathfinding.Services;

public class PathfindingPriorityService : IPathfindingPriorityService
{
    public IEnumerable<string> GetPriority()
    {
        return
        [
            NavmeshPathfindingProvider.Key,
        ];
    }
}
