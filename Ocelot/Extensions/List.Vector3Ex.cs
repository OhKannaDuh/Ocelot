using System;
using System.Collections.Generic;
using System.Numerics;

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

    public static List<Vector3> ContinueFrom(this List<Vector3> nodes, Vector3 position)
    {
        var result = new List<Vector3>();

        if (nodes.Count == 0)
        {
            return result;
        }

        if (nodes.Count == 1)
        {
            return [nodes[0]];
        }

        var bestIndex = 0;
        var bestScore = float.MaxValue;
        var bestProjected = nodes[0];

        for (var i = 0; i < nodes.Count - 1; i++)
        {
            var nodeA = nodes[i];
            var nodeB = nodes[i + 1];

            var projected = ClosestPointOnSegment(position, nodeA, nodeB);

            var cost = Vector3.Distance(position, projected);
            for (var j = i + 1; j < nodes.Count - 1; j++)
            {
                cost += Vector3.Distance(nodes[j], nodes[j + 1]);
            }

            if (cost < bestScore)
            {
                bestScore = cost;
                bestIndex = i;
                bestProjected = projected;
            }
        }

        result.Add(bestProjected);

        for (var i = bestIndex + 1; i < nodes.Count; i++)
        {
            result.Add(nodes[i]);
        }

        return result;
    }

    private static Vector3 ClosestPointOnSegment(Vector3 point, Vector3 a, Vector3 b)
    {
        var ab = b - a;
        var t = Vector3.Dot(point - a, ab) / Vector3.Dot(ab, ab);
        t = Math.Clamp(t, 0, 1);
        return a + t * ab;
    }
}
