using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Ocelot.Prowler;

public class ProwlerRuntime
{
    public required CancellationTokenSource Token { get; init; }

    public Task<List<Vector3>>? PathTask { get; set; }

    public bool PlannedFly { get; set; }

    public Vector3 LastPlannedDest { get; set; }

    public bool MovementHandedOff { get; set; }

    public event Action<ProwlContext>? Completed;

    public event Action<ProwlContext>? Cancelled;

    public event Action<ProwlContext>? Finally;


    public void FireCompleted(ProwlContext ctx)
    {
        Completed?.Invoke(ctx);
    }

    public void FireCancelled(ProwlContext ctx)
    {
        Cancelled?.Invoke(ctx);
    }

    public void FireFinally(ProwlContext ctx)
    {
        Finally?.Invoke(ctx);
    }
}
