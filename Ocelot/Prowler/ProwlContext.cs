using System.Collections.Generic;
using System.Numerics;
using Ocelot.Extensions;

namespace Ocelot.Prowler;

public class ProwlContext(Vector3 start, Vector3 destination, ProwlOptions options)
{
    public readonly ProwlOptions Options = options;

    public ProwlState State { get; internal set; } = ProwlState.NotStarted;

    public Vector3 OriginalStart { get; } = start;

    public Vector3 Start { get; set; } = start;

    public Vector3 OriginalDestination { get; } = destination;

    public Vector3 Destination { get; set; } = destination;

    public List<Vector3> OriginalNodes { get; internal set; } = [];

    public List<Vector3> Nodes { get; set; } = [];

    public float EuclideanDistance {
        get => Vector3.Distance(Start, Destination);
    }

    public float OriginalPathLength {
        get => OriginalNodes.Length();
    }

    public float PathLength {
        get => Nodes.Length();
    }
}
