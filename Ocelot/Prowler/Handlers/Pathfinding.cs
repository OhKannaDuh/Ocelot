using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Ocelot.Ipc;
using Ocelot.States;

namespace Ocelot.Prowler.Handlers;

[State<ProwlState>(ProwlState.Pathfinding)]
public class Pathfinding : Handler
{
    private Task<List<Vector3>>? pathfindingTask = null;

    public override void Enter(Prowl? prowl = null)
    {
        base.Enter(prowl);
        pathfindingTask?.Dispose();
        pathfindingTask = null;
    }

    public override void Exit(ProwlState nextState, Prowl? context = null)
    {
        base.Exit(nextState, context);
        pathfindingTask?.Dispose();
        pathfindingTask = null;
    }

    public override ProwlState? Handle(Prowl? prowl = null)
    {
        if (prowl == null || !VNavmesh.IsLoaded())
        {
            return ProwlState.Faulted;
        }

        var context = prowl.Context;
        pathfindingTask ??= VNavmesh.Pathfind(context.Start, context.Destination, prowl.Options.Movement.ShouldFly(context));

        if (pathfindingTask.IsCompleted)
        {
            if (pathfindingTask.IsFaulted || pathfindingTask.IsCanceled)
            {
                return ProwlState.Faulted;
            }

            prowl.Context.OriginalNodes = pathfindingTask.Result;
            prowl.Context.Nodes = pathfindingTask.Result;

            return ProwlState.Postprocessing;
        }

        return null;
    }
}
