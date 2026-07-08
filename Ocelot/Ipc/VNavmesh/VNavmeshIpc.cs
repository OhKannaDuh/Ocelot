using System.Numerics;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;

namespace Ocelot.Ipc.VNavmesh;

public class VNavmeshIpc(IDalamudPluginInterface plugin) : IVNavmeshIpc
{
    private readonly ICallGateSubscriber<bool> isPathfinding = plugin.GetIpcSubscriber<bool>("vnavmesh.Nav.PathfindInProgress");

    private readonly ICallGateSubscriber<bool> isRunning = plugin.GetIpcSubscriber<bool>("vnavmesh.Path.IsRunning");

    private readonly ICallGateSubscriber<Vector3, bool, object> pathfindAndMoveTo =
        plugin.GetIpcSubscriber<Vector3, bool, object>("vnavmesh.SimpleMove.PathfindAndMoveTo");

    private readonly ICallGateSubscriber<Vector3, bool, float, object> pathfindAndMoveCloseTo =
        plugin.GetIpcSubscriber<Vector3, bool, float, object>("vnavmesh.SimpleMove.PathfindAndMoveCloseTo");

    private readonly ICallGateSubscriber<Vector3, bool, float, Vector3?> findPointOnFloor =
        plugin.GetIpcSubscriber<Vector3, bool, float, Vector3?>("vnavmesh.Query.Mesh.PointOnFloor");

    private readonly ICallGateSubscriber<Vector3, float, float, Vector3?> findPointOnMesh =
        plugin.GetIpcSubscriber<Vector3, float, float, Vector3?>("vnavmesh.Query.Mesh.NearestPoint");

    private readonly ICallGateSubscriber<List<Vector3>> listWaypoints = plugin.GetIpcSubscriber<List<Vector3>>("vnavmesh.Path.ListWaypoints");

    private readonly ICallGateSubscriber<object> stop = plugin.GetIpcSubscriber<object>("vnavmesh.Path.Stop");

    private readonly ICallGateSubscriber<Vector3, Vector3, bool, Task<List<Vector3>>> pathfind =
        plugin.GetIpcSubscriber<Vector3, Vector3, bool, Task<List<Vector3>>>("vnavmesh.Nav.Pathfind");

    private readonly ICallGateSubscriber<Vector3, Vector3, bool, float, Task<List<Vector3>>> pathfindWithTolerance =
        plugin.GetIpcSubscriber<Vector3, Vector3, bool, float, Task<List<Vector3>>>("vnavmesh.Nav.PathfindWithTolerance");

    private readonly ICallGateSubscriber<Vector3, Vector3, bool, CancellationToken, Task<List<Vector3>>> pathfindCancelable =
        plugin.GetIpcSubscriber<Vector3, Vector3, bool, CancellationToken, Task<List<Vector3>>>("vnavmesh.Nav.PathfindCancelable");

    private readonly ICallGateSubscriber<List<Vector3>, bool, object> followPath =
        plugin.GetIpcSubscriber<List<Vector3>, bool, object>("vnavmesh.Path.MoveTo");

    public bool IsReady()
    {
        return pathfind.HasFunction
               && pathfindAndMoveTo.HasFunction
               && pathfindCancelable.HasFunction
               && stop.HasFunction;
    }

    public bool IsPathfinding()
    {
        return IsReady() && isPathfinding.HasFunction && isPathfinding.InvokeFunc();
    }

    public bool IsRunning()
    {
        return IsReady() && isRunning.HasFunction && isRunning.InvokeFunc();
    }

    public Task<List<Vector3>> Pathfind(Vector3 start, Vector3 end, bool fly, CancellationToken? cancel = null)
    {
        if (!IsReady())
        {
            return Task.FromResult(new List<Vector3>());
        }

        return cancel != null ? pathfindCancelable.InvokeFunc(start, end, fly, cancel.Value) : pathfind.InvokeFunc(start, end, fly);
    }

    public Task<List<Vector3>> Pathfind(Vector3 start, Vector3 end, bool fly, float range)
    {
        if (!IsReady())
        {
            return Task.FromResult(new List<Vector3>());
        }

        return pathfindWithTolerance.InvokeFunc(start, end, fly, range);
    }

    public void FollowPath(List<Vector3> path, bool fly)
    {
        if (!IsReady())
        {
            return;
        }

        followPath.InvokeAction(path, fly);
    }

    public void PathfindAndMoveTo(Vector3 destination, bool shouldFly)
    {
        if (!IsReady())
        {
            return;
        }

        pathfindAndMoveTo.InvokeFunc(destination, shouldFly);
    }

    public void PathfindAndMoveCloseTo(Vector3 destination, bool shouldFly, float range)
    {
        if (!IsReady())
        {
            return;
        }

        pathfindAndMoveCloseTo.InvokeFunc(destination, shouldFly, range);
    }

    public Vector3 FindPointOnFloor(Vector3 origin, float halfExtentXZ)
    {
        if (!findPointOnFloor.HasFunction)
        {
            return origin;
        }

        return findPointOnFloor.InvokeFunc(origin, false, halfExtentXZ) ?? origin;
    }

    public Vector3 FindPointOnMesh(Vector3 origin, float halfExtentXZ, float halfExtentY)
    {
        if (!findPointOnMesh.HasFunction)
        {
            return origin;
        }

        return findPointOnMesh.InvokeFunc(origin, halfExtentXZ, halfExtentY) ?? origin;
    }

    public List<Vector3> GetActiveNodes()
    {
        if (!listWaypoints.HasFunction)
        {
            return [];
        }

        return listWaypoints.InvokeFunc();
    }

    public void Stop()
    {
        if (!stop.HasFunction)
        {
            return;
        }

        stop.InvokeAction();
    }
}
