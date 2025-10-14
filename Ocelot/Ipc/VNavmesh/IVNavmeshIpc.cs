using System.Numerics;

namespace Ocelot.Ipc.VNavmesh;

public interface IVNavmeshIpc
{
    bool IsPathfinding();

    bool IsRunning();

    void PathfindAndMoveTo(Vector3 destination, bool shouldFly);

    void PathfindAndMoveCloseTo(Vector3 destination, bool shouldFly, float range);

    Vector3 FindPointOnFloor(Vector3 origin, float halfExtentXZ);

    List<Vector3> GetActiveNodes();

    void Stop();
}
