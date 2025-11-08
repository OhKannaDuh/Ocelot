using System.Numerics;

namespace Ocelot.Extensions;

public static class Vector2Extensions
{
    public static Vector3 Extend(this Vector2 vector, float extend = 0f)
    {
        // In ffxiv Y is up/down
        return new Vector3(vector.X, extend, vector.Y);
    }

    public static float Distance(this Vector2 vector, Vector2 other)
    {
        return Vector2.Distance(vector, other);
    }

    public static float Distance(this Vector2 vector, Vector3 other)
    {
        return Vector2.Distance(vector, other.Truncate());
    }

    public static Vector2 Normalized(this Vector2 v)
    {
        var length = v.Length();
        if (length > 0)
        {
            return v / length;
        }

        return Vector2.Zero;
    }
}
