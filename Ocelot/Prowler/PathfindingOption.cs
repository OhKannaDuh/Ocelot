using System.Collections.Generic;
using System.Numerics;
using Ocelot.Extensions;

namespace Ocelot.Prowler;

public readonly struct PathfindingOption(List<Vector3> nodes)
{
    public readonly List<Vector3> Nodes = nodes;

    public float Distance
    {
        get => Nodes.Length();
    }
}
