using ECommons.GameHelpers;
using Ocelot.Gameplay;
using Ocelot.States;

namespace Ocelot.Prowler.Handlers;

[State<ProwlState>(ProwlState.PreparingMovement)]
public class PreparingMovement : Handler
{
    public override ProwlState? Handle(Prowl? prowl = null)
    {
        if (prowl == null)
        {
            return ProwlState.Faulted;
        }

        var context = prowl.Context;
        var movement = prowl.Options.Movement;

        if (movement.ShouldFly(context) || movement.ShouldMount(context))
        {
            return ProwlState.Mount;
        }

        if (movement.ShouldSprint(context) && Actions.Sprint.CanCast() && !Player.Mounted)
        {
            Actions.Sprint.Cast();
        }

        return ProwlState.Moving;
    }
}
