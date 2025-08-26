using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;

namespace Ocelot.Extensions;

public static class ListVector3Ex
{
    public static float Length(this List<Vector3> nodes)
    {
        if (nodes.Count < 2)
        {
            return 0f;
        }

        var length = 0f;
        for (var i = 1; i < nodes.Count; i++)
        {
            length += Vector3.Distance(nodes[i - 1], nodes[i]);
        }

        return length;
    }

    public static List<Vector3> ContinueFrom(this List<Vector3> nodes, Vector3 point)
    {
        if (nodes.Count == 0)
        {
            return [];
        }

        var bestIndex = 0;
        var bestDistance = float.MaxValue;

        for (var i = 0; i < nodes.Count; i++)
        {
            var totalDistance = Vector3.Distance(point, nodes[i]);

            for (var j = i; j < nodes.Count - 1; j++)
            {
                totalDistance += Vector3.Distance(nodes[j], nodes[j + 1]);
            }

            if (totalDistance < bestDistance)
            {
                bestDistance = totalDistance;
                bestIndex = i;
            }
        }

        return nodes.GetRange(bestIndex, nodes.Count - bestIndex);
    }

    public static List<Vector3> Smooth(this List<Vector3> nodes, float pointsPerUnit = 0.25f, int minSegments = 2)
    {
        if (nodes.Count < 2)
        {
            return nodes;
        }

        var dedup = new List<Vector3>(nodes.Count) { nodes[0] };
        for (var k = 1; k < nodes.Count; k++)
        {
            if (Vector3.DistanceSquared(nodes[k], dedup[^1]) > float.Epsilon)
            {
                dedup.Add(nodes[k]);
            }
        }

        if (dedup.Count < 2)
        {
            return dedup;
        }

        static float SafeDiv(float num, float den)
        {
            return num / (MathF.Abs(den) < 1e-6f ? 1e-6f : den);
        }

        var smoothed = new List<Vector3>(dedup.Count * 4) { dedup[0] };

        for (var i = 0; i < dedup.Count - 1; i++)
        {
            var p1 = dedup[i];
            var p2 = dedup[i + 1];

            // Extrapolate endpoints to avoid zero-length parameter spans
            var p0 = i == 0 ? p1 + (p1 - p2) : dedup[i - 1];
            var p3 = i + 2 >= dedup.Count ? p2 + (p2 - p1) : dedup[i + 2];

            const float t0 = 0.0f;
            var t1 = GetT(t0, p0, p1);
            var t2 = GetT(t1, p1, p2);
            var t3 = GetT(t2, p2, p3);

            var segmentLength = Vector3.Distance(p1, p2);
            var segments = Math.Max(minSegments, (int)(segmentLength * pointsPerUnit));

            for (var j = 0; j <= segments; j++)
            {
                var t = t1 + (t2 - t1) * (j / (float)segments);

                var A1 = Lerp(p0, p1, SafeDiv(t1 - t, t1 - t0), SafeDiv(t - t0, t1 - t0));
                var A2 = Lerp(p1, p2, SafeDiv(t2 - t, t2 - t1), SafeDiv(t - t1, t2 - t1));
                var A3 = Lerp(p2, p3, SafeDiv(t3 - t, t3 - t2), SafeDiv(t - t2, t3 - t2));

                var B1 = Lerp(A1, A2, SafeDiv(t2 - t, t2 - t0), SafeDiv(t - t0, t2 - t0));
                var B2 = Lerp(A2, A3, SafeDiv(t3 - t, t3 - t1), SafeDiv(t - t1, t3 - t1));

                var C = Lerp(B1, B2, SafeDiv(t2 - t, t2 - t1), SafeDiv(t - t1, t2 - t1));

                smoothed.Add(C);
            }
        }

        if (smoothed.Count > 1 && Vector3.DistanceSquared(smoothed[^1], dedup[^1]) > 1e-10f)
        {
            smoothed.Add(dedup[^1]);
        }
        else
        {
            smoothed[^1] = dedup[^1];
        }

        return smoothed.Where(v => !(float.IsNaN(v.X) || float.IsNaN(v.Y) || float.IsNaN(v.Z))).ToList();
    }

    private static float GetT(float t, Vector3 p0, Vector3 p1)
    {
        return MathF.Pow(Vector3.Distance(p0, p1), 0.5f) + t;
    }

    private static Vector3 Lerp(Vector3 a, Vector3 b, float w1, float w2)
    {
        return a * w1 + b * w2;
    }

    public static List<Vector3> SnapToMesh(this List<Vector3> points)
    {
        var snapped = new List<Vector3>();

        foreach (var point in points)
        {
            var position = point;
            if (BGCollisionModule.RaycastMaterialFilter(position, new Vector3(0, 1, 0), out var hit))
            {
                position.Y = hit.Point.Y;
            }
            else
            {
                position.Y = Player.Position.Y + 250;
            }

            if (BGCollisionModule.RaycastMaterialFilter(position, new Vector3(0, -1, 0), out var hit2))
            {
                position.Y = hit2.Point.Y;
            }
            else
            {
                position.Y += 500;

                if (BGCollisionModule.RaycastMaterialFilter(position, new Vector3(0, -1, 0), out var hit3))
                {
                    position.Y = hit3.Point.Y;
                }
                else
                {
                    position.Y = point.Y;
                }
            }

            if (point.DistanceTo(position) > 5f)
            {
                continue;
            }

            snapped.Add(position);
        }

        return snapped;
    }

    public static Vector3 Center(this List<Vector3> points)
    {
        {
            if (points.Count == 0)
            {
                return Vector3.Zero;
            }

            var sum = Vector3.Zero;
            foreach (var point in points)
            {
                sum += point;
            }

            return sum / points.Count;
        }
    }
}
