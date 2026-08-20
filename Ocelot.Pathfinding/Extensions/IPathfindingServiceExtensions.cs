using Ocelot.Services.Pathfinding;

namespace Ocelot.Pathfinding.Extensions;

public static class IPathfindingServiceExtensions
{
    public static bool IsIdle(this IPathfinder service)
    {
        return service.GetState() == PathfindingState.Idle;
    }
}
