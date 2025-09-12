namespace Ocelot.Lifecycle;

public interface IOnStop : IOrderedHook
{
    void OnStop();
}
