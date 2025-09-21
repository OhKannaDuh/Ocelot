using Ocelot.Ipc.VNavmesh;
using Ocelot.Services.ClientState;
using Ocelot.Services.Pathfinding;
using Path = Ocelot.Services.Pathfinding.Path;

namespace Ocelot.Pathfinding.Services.Navmesh;

public class NavmeshPathfinder(
    IVNavmeshIpc nav,
    IClient client
) : IPathfinder
{
    public PathfindingState GetState()
    {
        if (nav.IsPathfinding())

        {
            return PathfindingState.Pathfinding;
        }

        if (nav.IsRunning())
        {
            return PathfindingState.Moving;
        }

        return PathfindingState.Idle;
    }

    public void PathfindAndMoveTo(Path path)
    {
        if (path.TerritoryType.HasValue && path.TerritoryType.Value.RowId != client.CurrentTerritoryId)
        {
            throw new InvalidOperationException("NavmeshPathfindingService does not support moving between territories");
        }

        var destination = path.To;
        if (path.ShouldSnapToFloor)
        {
            destination = nav.FindPointOnFloor(destination, path.FloorSnapExtents);
        }

        if (path.DistanceThreshold > 0f)
        {
            nav.PathfindAndMoveCloseTo(destination, path.AllowFlying, path.DistanceThreshold);
        }
        else
        {
            nav.PathfindAndMoveTo(destination, path.AllowFlying);
        }
    }

    public void Stop()
    {
        nav.Stop();
    }
}
