using Ocelot.States;

namespace Ocelot.Prowler.Handlers;

[State<ProwlState>(ProwlState.Preprocessing)]
public class Preprocessing : Handler
{
    public override ProwlState? Handle(Prowl? prowl = null)
    {
        if (prowl == null)
        {
            return ProwlState.Faulted;
        }

        prowl.Options.Preprocessor.Preprocess(prowl.Context);

        return ProwlState.Pathfinding;
    }
}
