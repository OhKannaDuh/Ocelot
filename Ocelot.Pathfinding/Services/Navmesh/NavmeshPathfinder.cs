using System.Numerics;
using Dalamud.Plugin.Services;
using Ocelot.Ipc.VNavmesh;
using Ocelot.Services.ClientState;
using Ocelot.Services.Pathfinding;
using Ocelot.Services.PlayerState;
using Path = Ocelot.Services.Pathfinding.Path;

namespace Ocelot.Pathfinding.Services.Navmesh;

public class NavmeshPathfinder(
    IVNavmeshIpc nav,
    IClient client,
    IPlayer player,
    IFramework framework
) : IPathfinder
{
    private CancellationTokenSource? moveCts;

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

    public void PathfindAndMoveTo(PathfinderConfig config)
    {
        if (!nav.IsNavmeshReady())
        {
            return;
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

        // vnavmesh's SimpleMove cannot be cancelled while it is still planning (a Stop then walks off anyway),
        // so plan with a cancelable query and hand the finished path over ourselves.
        moveCts?.Cancel();
        moveCts = new CancellationTokenSource();
        _ = MoveWhenPlanned(player.GetPosition(), destination, config.AllowFlying, config.DistanceThreshold, moveCts.Token);
    }

    private async Task MoveWhenPlanned(Vector3 start, Vector3 destination, bool fly, float range, CancellationToken token)
    {
        try
        {
            List<Vector3> points = await nav.Pathfind(start, destination, fly, token).WaitAsync(PathfindTimeout, token);
            points = TrimToRange(points, destination, range);
            await framework.RunOnFrameworkThread(() =>
            {
                if (!token.IsCancellationRequested && points.Count > 0)
                {
                    nav.FollowPath(points, fly);
                }
            });
        }
        catch (Exception)
        {
            // Cancelled, timed out or vnavmesh failed: nothing to walk.
        }
    }

    private static List<Vector3> TrimToRange(List<Vector3> points, Vector3 destination, float range)
    {
        if (range <= 0f)
        {
            return points;
        }

        int keep = points.FindIndex(p => Vector3.Distance(p, destination) <= range);
        return keep < 0 ? points : points.Take(keep + 1).ToList();
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
        moveCts?.Cancel();
        if (!nav.IsAvailable())
        {
            return;
        }

        nav.Stop();
    }
}
