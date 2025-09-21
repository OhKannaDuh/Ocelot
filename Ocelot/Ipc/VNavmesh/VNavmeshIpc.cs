using System.Numerics;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;

namespace Ocelot.Ipc.VNavmesh;

public class VNavmeshIpc(IDalamudPluginInterface plugin) : IVNavmeshIpc
{
    private readonly ICallGateSubscriber<bool> isPathfinding = plugin.GetIpcSubscriber<bool>("vnavmesh.Nav.PathfindInProgress");

    private readonly ICallGateSubscriber<bool> isRunning = plugin.GetIpcSubscriber<bool>("vnavmesh.Path.IsRunning");

    private readonly ICallGateSubscriber<Vector3, bool, object> pathfindAndMoveTo = plugin.GetIpcSubscriber<Vector3, bool, object>("vnavmesh.SimpleMove.PathfindAndMoveTo");

    private readonly ICallGateSubscriber<Vector3, bool, float, object> pathfindAndMoveCloseTo = plugin.GetIpcSubscriber<Vector3, bool, float, object>("vnavmesh.SimpleMove.PathfindAndMoveCloseTo");

    private readonly ICallGateSubscriber<Vector3, bool, float, Vector3?> findPointOnFloor = plugin.GetIpcSubscriber<Vector3, bool, float, Vector3?>("vnavmesh.Query.Mesh.PointOnFloor");

    private readonly ICallGateSubscriber<List<Vector3>> listWaypoints = plugin.GetIpcSubscriber<List<Vector3>>("vnavmesh.Path.ListWaypoints");
    
    private readonly ICallGateSubscriber<object> stop = plugin.GetIpcSubscriber<object>("vnavmesh.Path.Stop");

    public bool IsPathfinding()
    {
        return isPathfinding.HasFunction && isPathfinding.InvokeFunc();
    }

    public bool IsRunning()
    {
        return isRunning.HasFunction && isRunning.InvokeFunc();
    }

    public void PathfindAndMoveTo(Vector3 destination, bool shouldFly)
    {
        pathfindAndMoveTo.InvokeFunc(destination, shouldFly);
    }
    
    public void PathfindAndMoveCloseTo(Vector3 destination, bool shouldFly, float range)
    {
        pathfindAndMoveCloseTo.InvokeFunc(destination, shouldFly, range);
    }

    public Vector3 FindPointOnFloor(Vector3 origin, float halfExtentXZ)
    {
        return findPointOnFloor.InvokeFunc(origin, false, halfExtentXZ) ?? origin;
    }

    public List<Vector3> GetActiveNodes()
    {
        return listWaypoints.InvokeFunc();
    }

    public void Stop()
    {
        stop.InvokeAction();
    }
}
