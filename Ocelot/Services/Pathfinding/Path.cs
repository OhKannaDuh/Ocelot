using System.Numerics;
using Lumina.Excel.Sheets;

namespace Ocelot.Services.Pathfinding;

public class Path(Vector3 to)
{
    public Vector3 To { get; } = to;

    public Vector3 From { get; init; }

    public float DistanceThreshold { get; init; } = 0f;

    public bool AllowFlying { get; init; }

    public TerritoryType? TerritoryType { get; init; }

    public bool ShouldSnapToFloor { get; init; } = false;

    public float FloorSnapExtents { get; init; } = 5f;

    public static Path Fly(Vector3 to)
    {
        return new Path(to)
        {
            AllowFlying = true,
        };
    }
}
