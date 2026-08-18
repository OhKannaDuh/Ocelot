using System.Numerics;

namespace Ocelot.Services.Pathfinding;

public class Path
{
    public readonly IReadOnlyList<Vector3> Nodes;

    private readonly PathfinderConfig Config;

    public Path(IEnumerable<Vector3> path, PathfinderConfig config, IPathfinder pathfinder)
    {
        var nodes = new List<Vector3>();
        if (config.ShouldSnapToFloor)
        {
            foreach (var node in path)
            {
                nodes.Add(pathfinder.SnapToMesh(node, config.FloorSnapExtents));
            }
        }
        else
        {
            nodes.AddRange(path);
        }

        Nodes = nodes;
        Config = config with { ShouldSnapToFloor = false };
    }

    public bool ShouldFly
    {
        get => Config.AllowFlying;
    }

    public float Distance
    {
        get
        {
            if (Nodes.Count < 2)
            {
                return 0f;
            }

            var sum = 0f;
            for (var i = 1; i < Nodes.Count; i++)
            {
                sum += Vector3.Distance(Nodes[i - 1], Nodes[i]);
            }

            return sum;
        }
    }

    public static Path Blank(IPathfinder pathfinder)
    {
        return new Path([], new PathfinderConfig(Vector3.NaN), pathfinder);
    }
}
