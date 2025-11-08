using System.Numerics;
using Ocelot.Ipc.VNavmesh;
using Ocelot.Services.ClientState;
using Ocelot.Services.Pathfinding;
using Ocelot.Services.PlayerState;
using Path = Ocelot.Services.Pathfinding.Path;

namespace Ocelot.Pathfinding.Services.Navmesh;

public class NavmeshPathfinder(
    IVNavmeshIpc nav,
    IClient client,
    IPlayer player
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

    public void PathfindAndMoveTo(PathfinderConfig config)
    {
        if (config.TerritoryType.HasValue && config.TerritoryType.Value.RowId != client.CurrentTerritoryId)
        {
            throw new InvalidOperationException("NavmeshPathfindingService does not support moving between territories");
        }

        var destination = config.To();
        if (config.ShouldSnapToFloor)
        {
            destination = nav.FindPointOnFloor(destination, config.FloorSnapExtents);
        }

        if (config.DistanceThreshold > 0f)
        {
            nav.PathfindAndMoveCloseTo(destination, config.AllowFlying, config.DistanceThreshold);
        }
        else
        {
            nav.PathfindAndMoveTo(destination, config.AllowFlying);
        }
    }

    public async Task<Path> Pathfind(PathfinderConfig config)
    {
        Task<List<Vector3>> task;

        var start = config.From ?? player.GetPosition();
        if (config.DistanceThreshold > 0f)
        {
            task = nav.Pathfind(start, config.To(), config.AllowFlying, config.DistanceThreshold);
        }
        else
        {
            task = nav.Pathfind(start, config.To(), config.AllowFlying);
        }

        var points = await task.ConfigureAwait(false);

        return new Path(points, config, this);
    }

    public void FollowPath(Path path)
    {
        nav.FollowPath(path.Nodes.ToList(), path.ShouldFly);
    }

    public Vector3 SnapToMesh(Vector3 point, float extent)
    {
        return nav.FindPointOnFloor(point, extent);
    }

    public void Stop()
    {
        nav.Stop();
    }
}
