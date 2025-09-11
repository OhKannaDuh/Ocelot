using System.Numerics;

namespace Ocelot.Services.Pathfinding;

[OcelotService(typeof(IPathfinderService))]
public class PathfinderService : IPathfinderService
{
    private IPathfinderService current = new BlankPathfinderService();

    public void SetPathfinderService(IPathfinderService service)
    {
        current = service;
    }

    public bool IsValid()
    {
        return current.IsValid();
    }

    public PathfinderState GetState()
    {
        return current.GetState();
    }

    public void Pathfind(Vector3 from, Vector3 to, PathfindingConfig? config = null)
    {
        current.Pathfind(from, to, config);
    }

    public void Stop()
    {
        current.Stop();
    }

    public void RenderDebug()
    {
        current.RenderDebug();
    }
}
