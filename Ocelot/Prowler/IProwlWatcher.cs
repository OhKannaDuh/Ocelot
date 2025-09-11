namespace Ocelot.Prowler;

public interface IProwlWatcher
{
    bool ShouldStop(ProwlContext context);
}
