using Ocelot.Services;

namespace Ocelot.Prowler;

[OcelotService(typeof(IProwlWatcher))]
public class DefaultProwlWatcher : IProwlWatcher
{
    public bool ShouldStop(ProwlContext context)
    {
        return false;
    }
}
