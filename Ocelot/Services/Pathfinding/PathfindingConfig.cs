namespace Ocelot.Services.Pathfinding;

public class PathfindingConfig
{
    public bool ShouldSprint { get; init; } = false;

    public bool ShouldMount { get; init; } = false;

    public bool ShouldFly { get; init; } = false;

    public float ArrivalRadius { get; init; } = 0.4f;

    public static PathfindingConfig Sprint()
    {
        return new PathfindingConfig {
            ShouldSprint = true,
        };
    }

    public static PathfindingConfig Mount()
    {
        return new PathfindingConfig {
            ShouldMount = true,
        };
    }

    public static PathfindingConfig Fly()
    {
        return new PathfindingConfig {
            ShouldMount = true,
            ShouldFly = true,
        };
    }
}
