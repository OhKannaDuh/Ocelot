using System.Numerics;

namespace Ocelot.Extensions;

public static class Vector3Extensions
{
    public static Vector2 Truncate(this Vector3 vector)
    {
        // In ffxiv Y is up/down
        return new Vector2(vector.X, vector.Z);
    }

    public static float Distance(this Vector3 vector, Vector3 other)
    {
        return Vector3.Distance(vector, other);
    }

    public static float Distance2D(this Vector3 vector, Vector2 other)
    {
        return Vector2.Distance(vector.Truncate(), other);
    }

    public static float Distance2D(this Vector3 vector, Vector3 other)
    {
        return Vector2.Distance(vector.Truncate(), other.Truncate());
    }

    public static Vector3 GetApproachPosition(this Vector3 to, Vector3 from, float range = 3f, float jitter = 0f)
    {
        var distance = from.Distance(to);
        if (distance <= range)
        {
            return from;
        }

        var direction = to - from;
        if (direction.LengthSquared() < 0.0001f)
        {
            return to;
        }

        direction /= distance;

        if (jitter > 0f)
        {
            var angleDeg = Random.Shared.NextSingle() * 2f * jitter - jitter;
            var angleRad = MathF.PI / 180f * angleDeg;

            var cos = MathF.Cos(angleRad);
            var sin = MathF.Sin(angleRad);

            direction = new Vector3(
                direction.X * cos - direction.Z * sin,
                direction.Y,
                direction.X * sin + direction.Z * cos
            );
        }

        return to - direction * range;
    }


    public static Vector3 Centroid(this IEnumerable<Vector3> positions)
    {
        var count = 0;
        var sum = Vector3.Zero;

        foreach (var pos in positions)
        {
            sum += pos;
            count++;
        }

        if (count == 0)
        {
            return sum;
        }

        return sum / count;
    }

    public static Vector2 Centroid(this IEnumerable<Vector2> positions)
    {
        var count = 0;
        var sum = Vector2.Zero;

        foreach (var pos in positions)
        {
            sum += pos;
            count++;
        }

        if (count == 0)
        {
            return sum;
        }

        return sum / count;
    }
}
