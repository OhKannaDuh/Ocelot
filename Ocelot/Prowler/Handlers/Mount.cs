using ECommons.GameHelpers;
using ECommons.Throttlers;
using Ocelot.Gameplay;
using Ocelot.States;

namespace Ocelot.Prowler.Handlers;

[State<ProwlState>(ProwlState.Mount)]
public class Mount : Handler
{
    public override ProwlState? Handle(Prowl? prowl = null)
    {
        if (prowl == null)
        {
            return ProwlState.Faulted;
        }

        if (Player.Mounted)
        {
            return ProwlState.Moving;
        }

        if (EzThrottler.Throttle("Prowl.Mount") && !Player.Mounted)
        {
            if (Actions.Mount(prowl.Options.MountId).CanCast())
            {
                Actions.Mount(prowl.Options.MountId).Cast();
            }
        }

        return null;
    }
}
