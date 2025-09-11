using Ocelot.Services;

namespace Ocelot.Prowler;

[OcelotService(typeof(IPathPreprocessor))]
public class DefaultPathPreprocessor : IPathPreprocessor
{
    public void Preprocess(ProwlContext context) { }
}
