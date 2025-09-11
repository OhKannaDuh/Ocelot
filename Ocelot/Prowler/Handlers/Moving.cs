using ECommons.GameHelpers;
using Ocelot.Extensions;
using Ocelot.Ipc;
using Ocelot.States;

namespace Ocelot.Prowler.Handlers;

[State<ProwlState>(ProwlState.Moving)]
public class Moving : Handler
{
    public override ProwlState? Handle(Prowl? prowl = null)
    {
        if (prowl == null || !VNavmesh.IsLoaded())
        {
            return ProwlState.Faulted;
        }

        var context = prowl.Context;
        var movement = prowl.Options.Movement;

        if (!VNavmesh.IsRunning())
        {
            var path = context.Nodes.ContinueFrom(Player.Position);
            VNavmesh.FollowPath(path, movement.ShouldFly(context));
            return null;
        }

        if (Player.Position.DistanceTo2D(prowl.Context.Destination) <= prowl.Options.ArrivalRadius)
        {
            LogDebug("Arrived at destination");
            VNavmesh.Stop();
            return ProwlState.Complete;
        }

        if (prowl.Options.Watcher.ShouldStop(context))
        {
            LogDebug("Stopped by Watcher");
            VNavmesh.Stop();
            return ProwlState.Complete;
        }

        if (prowl.Options.TargetProvider != null)
        {
            var position = prowl.Options.TargetProvider.GetCurrentPosition(context);
            var distance = prowl.Context.Destination.DistanceTo2D(position);

            if (distance >= prowl.Options.RetargetTolerance)
            {
                LogDebug("Target moved, redirecting");
                prowl.Context.Destination = position;
                return ProwlState.Redirecting;
            }
        }

        if (prowl.Options.Interceptor != null)
        {
            if (prowl.Options.Interceptor.TryIntercept(context))
            {
                LogDebug("Path was intercepted and changed");
                var path = context.Nodes.ContinueFrom(Player.Position);
                ;
                VNavmesh.FollowPath(path, movement.ShouldFly(context));
            }
        }

        return null;
    }
}
