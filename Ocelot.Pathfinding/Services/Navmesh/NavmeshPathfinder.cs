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
    private static readonly TimeSpan PathfindTimeout = TimeSpan.FromSeconds(30);

    public PathfindingState GetState()
    {
        if (!nav.IsAvailable())
        {
            return PathfindingState.Idle;
        }

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

    public bool PathfindAndMoveTo(PathfinderConfig config)
    {
        if (!nav.IsNavmeshReady())
        {
            return false;
        }

        if (config.TerritoryType.HasValue && config.TerritoryType.Value.RowId != client.CurrentTerritoryId)
        {
            throw new InvalidOperationException("NavmeshPathfindingService does not support moving between territories");
        }

        var destination = config.To();
        if (config.ShouldSnapToFloor)
        {
            destination = nav.FindPointOnFloor(destination, config.FloorSnapExtents);
        }

        return config.DistanceThreshold > 0f
            ? nav.PathfindAndMoveCloseTo(destination, config.AllowFlying, config.DistanceThreshold)
            : nav.PathfindAndMoveTo(destination, config.AllowFlying);
    }

    public async Task<Path> Pathfind(PathfinderConfig config)
    {
        if (!nav.IsNavmeshReady())
        {
            return new Path([], config, this);
        }

        Task<List<Vector3>> task;

        var start = config.From ?? player.GetPosition();
        var destination = config.To();
        if (config.ShouldSnapToFloor)
        {
            destination = nav.FindPointOnFloor(destination, config.FloorSnapExtents);
        }

        if (config.DistanceThreshold > 0f)
        {
            task = nav.Pathfind(start, destination, config.AllowFlying, config.DistanceThreshold);
        }
        else
        {
            task = nav.Pathfind(start, destination, config.AllowFlying);
        }

        try
        {
            var points = await task.WaitAsync(PathfindTimeout).ConfigureAwait(false);
            return new Path(points, config, this);
        }
        // vnavmesh cancels or fails a query on a mesh reload or zone change; callers treat an empty path as unreachable.
        catch (Exception)
        {
            return new Path([], config, this);
        }
    }

    public void FollowPath(Path path)
    {
        if (!nav.IsNavmeshReady())
        {
            return;
        }

        nav.FollowPath(path.Nodes.ToList(), path.ShouldFly);
    }

    public Vector3 SnapToMesh(Vector3 point, float extent)
    {
        return nav.FindPointOnFloor(point, extent);
    }

    public void Stop()
    {
        if (!nav.IsAvailable())
        {
            return;
        }

        nav.Stop();
    }
}
