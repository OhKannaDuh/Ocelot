using System.Numerics;

namespace Ocelot.Services.Pathfinding;

public interface IPathfinderService
{
    bool IsValid();

    PathfinderState GetState();

    void Pathfind(Vector3 from, Vector3 to, PathfindingConfig? config = null);

    void Stop();

    void RenderDebug();

    bool IsRunning {
        get {
            var state = GetState();
            return state is PathfinderState.Running or PathfinderState.Pathfinding;
        }
    }
}
