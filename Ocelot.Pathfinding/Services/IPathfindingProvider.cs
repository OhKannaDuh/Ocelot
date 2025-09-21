using Ocelot.Services.Pathfinding;

namespace Ocelot.Pathfinding.Services;

public interface IPathfindingProvider
{
    string InternalName { get; }

    string DisplayName { get; }

    bool IsAvailable();

    IPathfinder Create();
}
