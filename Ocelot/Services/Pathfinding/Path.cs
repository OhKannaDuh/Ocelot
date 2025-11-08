using System.Numerics;

namespace Ocelot.Services.Pathfinding;

public class Path
{
    public readonly IReadOnlyList<Vector3> Nodes;

    private readonly PathfinderConfig Config;

    private readonly IPathfinder Pathfinder;

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
        Pathfinder = pathfinder;
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

    public Path Smoothed(float pointsPerUnit = 0.25f, int minSegments = 2, float epsilon = 1e-4f, float alpha = 0.5f)
    {
        if (Nodes.Count < 2)
        {
            return this;
        }

        var points = new List<Vector3>(Nodes.Count);
        Vector3? prev = null;
        foreach (var n in Nodes)
        {
            if (prev is null || !Near(n, prev.Value, epsilon))
            {
                points.Add(n);
                prev = n;
            }
        }

        if (points.Count < 2)
        {
            return new Path(points, Config, Pathfinder);
        }

        if (points.Count == 2)
        {
            var p1 = points[0];
            var p2 = points[1];
            var segLen = Vector3.Distance(p1, p2);
            var segments = Math.Max(minSegments, (int)MathF.Ceiling(segLen * pointsPerUnit));

            var out2 = new List<Vector3>(segments + 1) { p1 };
            for (var j = 1; j <= segments; j++)
            {
                var u = j / (float)segments;
                out2.Add(Vector3.Lerp(p1, p2, u));
            }

            return new Path(out2, Config, Pathfinder);
        }

        var smoothed = new List<Vector3>(points.Count * 4) { points[0] };

        for (var i = 0; i < points.Count - 1; i++)
        {
            var p0 = i > 0 ? points[i - 1] : points[i];
            var p1 = points[i];
            var p2 = points[i + 1];
            var p3 = i + 2 < points.Count ? points[i + 2] : points[i + 1];

            var t0 = 0f;
            var t1 = GetT(t0, p0, p1);
            var t2 = GetT(t1, p1, p2);
            var t3 = GetT(t2, p2, p3);

            var segLen = MathF.Max(Vector3.Distance(p1, p2), epsilon);
            var segments = Math.Max(minSegments, (int)MathF.Ceiling(segLen * pointsPerUnit));

            // Avoid duplicating junction point: start at j=1
            for (var j = 1; j <= segments; j++)
            {
                var t = t1 + (t2 - t1) * (j / (float)segments);

                float d10 = t1 - t0,
                    d21 = t2 - t1,
                    d32 = t3 - t2,
                    d20 = t2 - t0,
                    d31 = t3 - t1;
                var degenerate = d10 < epsilon || d21 < epsilon || d32 < epsilon || d20 < epsilon || d31 < epsilon;

                if (degenerate)
                {
                    // Fallback to straight interpolation on this segment
                    var u = (t - t1) / MathF.Max(t2 - t1, epsilon);
                    smoothed.Add(Vector3.Lerp(p1, p2, u));
                    continue;
                }

                var A1 = (t1 - t) / d10 * p0 + (t - t0) / d10 * p1;
                var A2 = (t2 - t) / d21 * p1 + (t - t1) / d21 * p2;
                var A3 = (t3 - t) / d32 * p2 + (t - t2) / d32 * p3;

                var B1 = (t2 - t) / d20 * A1 + (t - t0) / d20 * A2;
                var B2 = (t3 - t) / d31 * A2 + (t - t1) / d31 * A3;

                var d21b = t2 - t1;
                if (d21b < epsilon)
                {
                    var u = (t - t1) / MathF.Max(t2 - t1, epsilon);
                    smoothed.Add(Vector3.Lerp(p1, p2, u));
                }
                else
                {
                    var C = (t2 - t) / d21b * B1 + (t - t1) / d21b * B2;
                    smoothed.Add(C);
                }
            }
        }

        // Ensure exact last point present once
        if (!Near(smoothed[^1], points[^1], epsilon))
        {
            smoothed.Add(points[^1]);
        }

        return new Path(smoothed, Config, Pathfinder);

        static bool Near(Vector3 a, Vector3 b, float eps)
        {
            return Vector3.DistanceSquared(a, b) <= eps * eps;
        }

        float GetT(float t, Vector3 p0, Vector3 p1)
        {
            var d = Vector3.Distance(p0, p1);
            if (d < epsilon)
            {
                d = epsilon;
            }

            return t + MathF.Pow(d, alpha);
        }
    }


    public Path From(Vector3 point)
    {
        if (Nodes.Count == 0)
        {
            return this;
        }

        if (Nodes.Count == 1)
        {
            return new Path([point, Nodes[0]], Config, Pathfinder);
        }

        var closest = Nodes
            .Select((node, index) => new
            {
                Index = index,
                Node = node,
                Dist = Vector3.Distance(node, point),
            })
            .OrderBy(x => x.Dist)
            .Take(2)
            .ToList();

        var bestDistance = float.MaxValue;
        var bestIndex = 0;

        foreach (var c in closest)
        {
            var subPathNodes = Nodes.Skip(c.Index);
            var partialPath = new Path(subPathNodes, Config, Pathfinder);

            if (partialPath.Distance < bestDistance)
            {
                bestDistance = partialPath.Distance;
                bestIndex = c.Index;
            }
        }

        var resultNodes = Nodes.Skip(bestIndex);

        return new Path(resultNodes, Config, Pathfinder);
    }

    public static Path Blank(IPathfinder pathfinder)
    {
        return new Path([], new PathfinderConfig(Vector3.NaN), pathfinder);
    }
}
