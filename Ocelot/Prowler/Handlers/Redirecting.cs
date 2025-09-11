using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using ECommons.GameHelpers;
using Ocelot.Extensions;
using Ocelot.Ipc;
using Ocelot.States;

namespace Ocelot.Prowler.Handlers;

[State<ProwlState>(ProwlState.Redirecting)]
public class Redirecting : Handler
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

            prowl.Context.Nodes = pathfindingTask.Result;
            prowl.Options.Postprocessor.Postprocess(context);

            var path = context.Nodes.ContinueFrom(Player.Position);
            ;
            VNavmesh.FollowPath(path, prowl.Options.Movement.ShouldFly(context));

            return ProwlState.Moving;
        }

        return null;
    }
}
