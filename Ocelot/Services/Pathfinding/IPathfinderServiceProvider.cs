namespace Ocelot.Services.Pathfinding;

public interface IPathfinderServiceProvider
{
    IPathfinderService GetService();
}
