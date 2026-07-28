using System.Numerics;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Ocelot.Services.PluginStatus;

namespace Ocelot.Ipc.VNavmesh;

public class VNavmeshIpc(IPluginStatus pluginStatus, IDalamudPluginInterface plugin) : IVNavmeshIpc
{
    private const string PluginName = "vnavmesh";
    private readonly ICallGateSubscriber<bool> navIsReady = plugin.GetIpcSubscriber<bool>("vnavmesh.Nav.IsReady");

    private readonly ICallGateSubscriber<bool> isPathfinding = plugin.GetIpcSubscriber<bool>("vnavmesh.Nav.PathfindInProgress");

    private readonly ICallGateSubscriber<bool> simpleMovePathfindInProgress =
        plugin.GetIpcSubscriber<bool>("vnavmesh.SimpleMove.PathfindInProgress");

    private readonly ICallGateSubscriber<bool> isRunning = plugin.GetIpcSubscriber<bool>("vnavmesh.Path.IsRunning");

    private readonly ICallGateSubscriber<Vector3, bool, bool> pathfindAndMoveTo =
        plugin.GetIpcSubscriber<Vector3, bool, bool>("vnavmesh.SimpleMove.PathfindAndMoveTo");

    private readonly ICallGateSubscriber<Vector3, bool, float, bool> pathfindAndMoveCloseTo =
        plugin.GetIpcSubscriber<Vector3, bool, float, bool>("vnavmesh.SimpleMove.PathfindAndMoveCloseTo");

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

    public bool IsAvailable()
    {
        return pluginStatus.IsLoaded(PluginName);
    }

    public bool IsNavmeshReady()
    {
        if (!IsAvailable() || !navIsReady.HasFunction)
        {
            return false;
        }

        try
        {
            return navIsReady.InvokeFunc();
        }
        catch
        {
            return false;
        }
    }

    public bool IsPathfinding()
    {
        if (!IsAvailable())
        {
            return false;
        }

        try
        {
            if (simpleMovePathfindInProgress.HasFunction && simpleMovePathfindInProgress.InvokeFunc())
            {
                return true;
            }

            return isPathfinding.HasFunction && isPathfinding.InvokeFunc();
        }
        catch
        {
            return false;
        }
    }

    public bool IsRunning()
    {
        if (!IsAvailable())
        {
            return false;
        }

        try
        {
            return isRunning.HasFunction && isRunning.InvokeFunc();
        }
        catch
        {
            return false;
        }
    }

    public Task<List<Vector3>> Pathfind(Vector3 start, Vector3 end, bool fly, CancellationToken? cancel = null)
    {
        if (!pathfind.HasFunction)
        {
            return Task.FromResult(new List<Vector3>());
        }

        try
        {
            return cancel != null
                ? pathfindCancelable.InvokeFunc(start, end, fly, cancel.Value)
                : pathfind.InvokeFunc(start, end, fly);
        }
        catch
        {
            return Task.FromResult(new List<Vector3>());
        }
    }

    public Task<List<Vector3>> Pathfind(Vector3 start, Vector3 end, bool fly, float range)
    {
        if (!pathfindWithTolerance.HasFunction)
        {
            return Task.FromResult(new List<Vector3>());
        }

        try
        {
            return pathfindWithTolerance.InvokeFunc(start, end, fly, range);
        }
        catch
        {
            return Task.FromResult(new List<Vector3>());
        }
    }

    public void FollowPath(List<Vector3> path, bool fly)
    {
        if (!followPath.HasFunction)
        {
            return;
        }

        try
        {
            followPath.InvokeAction(path, fly);
        }
        catch
        {
            // ignored
        }
    }

    public void PathfindAndMoveTo(Vector3 destination, bool shouldFly)
    {
        if (!pathfindAndMoveTo.HasFunction)
        {
            return;
        }

        try
        {
            pathfindAndMoveTo.InvokeFunc(destination, shouldFly);
        }
        catch
        {
            // ignored
        }
    }

    public void PathfindAndMoveCloseTo(Vector3 destination, bool shouldFly, float range)
    {
        if (!pathfindAndMoveCloseTo.HasFunction)
        {
            return;
        }

        try
        {
            pathfindAndMoveCloseTo.InvokeFunc(destination, shouldFly, range);
        }
        catch
        {
            // ignored
        }
    }

    public Vector3 FindPointOnFloor(Vector3 origin, float halfExtentXZ)
    {
        if (!findPointOnFloor.HasFunction)
        {
            return origin;
        }

        try
        {
            return findPointOnFloor.InvokeFunc(origin, false, halfExtentXZ) ?? origin;
        }
        catch
        {
            return origin;
        }
    }

    public Vector3 FindPointOnMesh(Vector3 origin, float halfExtentXZ, float halfExtentY)
    {
        if (!findPointOnMesh.HasFunction)
        {
            return origin;
        }

        try
        {
            return findPointOnMesh.InvokeFunc(origin, halfExtentXZ, halfExtentY) ?? origin;
        }
        catch
        {
            return origin;
        }
    }

    public List<Vector3> GetActiveNodes()
    {
        if (!listWaypoints.HasFunction)
        {
            return [];
        }

        try
        {
            return listWaypoints.InvokeFunc();
        }
        catch
        {
            return [];
        }
    }

    public void Stop()
    {
        if (!IsAvailable() || !stop.HasAction)
        {
            return;
        }

        try
        {
            stop.InvokeAction();
        }
        catch
        {
            // ignored
        }
    }
}
