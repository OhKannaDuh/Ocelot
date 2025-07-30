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
}
