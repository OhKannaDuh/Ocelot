using System;
using System.Numerics;

namespace Ocelot.Extensions;

public static class Vector3Ex
{
    public static float Distance2D(this Vector3 a, Vector3 b)
    {
        var dx = a.X - b.X;
        var dz = a.Z - b.Z;
        return MathF.Sqrt(dx * dx + dz * dz);
    }

    public static float Distance(this Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }
}
