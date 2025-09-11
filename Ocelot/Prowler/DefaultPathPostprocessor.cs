using Ocelot.Services;

namespace Ocelot.Prowler;

[OcelotService(typeof(IPathPostprocessor))]
public class DefaultPathPostprocessor : IPathPostprocessor
{
    public void Postprocess(ProwlContext context) { }
}
