using System.Numerics;
using Ocelot.UI;

namespace Ocelot.Services.Pathfinding;

[OcelotService(typeof(BlankPathfinderService))]
public class BlankPathfinderService : IPathfinderService
{
    public bool IsValid()
    {
        return false;
    }

    public PathfinderState GetState()
    {
        throw new ServiceNotFoundException(typeof(IPathfinderService));
    }

    public void Pathfind(Vector3 from, Vector3 to, PathfindingConfig? config = null)
    {
        throw new ServiceNotFoundException(typeof(IPathfinderService));
    }

    public void Stop()
    {
        throw new ServiceNotFoundException(typeof(IPathfinderService));
    }

    public void RenderDebug()
    {
        OcelotUI.Error("No pathfinder service could be loaded.");
        ;
    }
}
