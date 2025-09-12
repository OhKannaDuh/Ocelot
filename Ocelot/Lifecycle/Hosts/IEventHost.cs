namespace Ocelot.Lifecycle.Hosts;

public interface IEventHost : IOrderedHook
{
    void Start();

    void Stop();

    int Count { get; }
}
