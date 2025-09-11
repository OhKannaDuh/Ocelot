namespace Ocelot.Prowler;

public interface IEventHandler
{
    void OnCancelled(ProwlContext context);

    void OnFaulted(ProwlContext context);

    void OnComplete(ProwlContext context);

    void Finally(ProwlContext context);
}
