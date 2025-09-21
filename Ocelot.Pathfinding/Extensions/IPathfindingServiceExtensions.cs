using Ocelot.Pathfinding.Services;
using Ocelot.Services.Pathfinding;

namespace Ocelot.Pathfinding.Extensions;

public static class IPathfindingServiceExtensions
{
    public static bool IsIdle(this IPathfinder service)
    {
        return service.GetState() == PathfindingState.Idle;
    }

    public static bool IsPathfinding(this IPathfinder service)
    {
        return service.GetState() == PathfindingState.Pathfinding;
    }

    public static bool IsMoving(this IPathfinder service)
    {
        return service.GetState() == PathfindingState.Moving;
    }
    
    
}
