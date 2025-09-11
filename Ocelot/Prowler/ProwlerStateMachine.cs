using Ocelot.States;

namespace Ocelot.Prowler;

public class ProwlerStateMachine(Prowl prowl) : StateMachine<ProwlState, Prowl>(ProwlState.NotStarted)
{
    protected override Prowl GetContext()
    {
        return prowl;
    }

    public Prowl GetProwl()
    {
        return prowl;
    }
}
