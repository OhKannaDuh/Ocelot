using Ocelot.States;

namespace Ocelot.Prowler.Handlers;

[State<ProwlState>(ProwlState.Postprocessing)]
public class Postprocessing : Handler
{
    public override ProwlState? Handle(Prowl? prowl = null)
    {
        if (prowl == null)
        {
            return ProwlState.Faulted;
        }

        prowl.Options.Postprocessor.Postprocess(prowl.Context);

        LogDebug("Preprocessor ran, Original Length {oglength}, {newlength}", prowl.Context.OriginalNodes.Count, prowl.Context.Nodes.Count);

        return ProwlState.PreparingMovement;
    }
}
