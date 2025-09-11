using Ocelot.Ipc;
using Ocelot.States;

namespace Ocelot.Prowler.Handlers;

[State<ProwlState>(ProwlState.NotStarted)]
public class NotStarted : Handler
{
    public override ProwlState? Handle(Prowl? prowl = null)
    {
        if (prowl == null)
        {
            return ProwlState.Faulted;
        }

        if (VNavmesh.IsRunning() || VNavmesh.IsPathfinding())
        {
            return null;
        }

        return ProwlState.Preprocessing;
    }
}
