using System.Numerics;
using Ocelot.Ipc;
using Ocelot.UI;

namespace Ocelot.Services.Pathfinding;

[OcelotService(typeof(NavmeshPathfinderService))]
public class NavmeshPathfinderService : IPathfinderService
{
    public static bool RequirementsMet()
    {
        return VNavmesh.IsLoaded();
    }

    public bool IsValid()
    {
        return RequirementsMet();
    }

    public PathfinderState GetState()
    {
        if (VNavmesh.IsPathfinding())
        {
            return PathfinderState.Pathfinding;
        }

        if (VNavmesh.IsRunning())
        {
            return PathfinderState.Running;
        }

        return PathfinderState.Idle;
    }

    public void Pathfind(Vector3 from, Vector3 to, PathfindingConfig? config = null)
    {
        VNavmesh.PathfindAndMoveTo(to, config?.ShouldFly ?? false);
    }

    public void Stop()
    {
        VNavmesh.Stop();
    }

    public void RenderDebug()
    {
        OcelotUI.LabelledValue("VNavmesh State", GetState());
    }
}
