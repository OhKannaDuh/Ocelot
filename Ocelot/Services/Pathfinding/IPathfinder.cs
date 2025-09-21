namespace Ocelot.Services.Pathfinding;

public interface IPathfinder
{
    PathfindingState GetState();

    void PathfindAndMoveTo(Path path);

    void Stop();
}
