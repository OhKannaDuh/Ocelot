using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public class StopHost(IEnumerable<IOnStop> stop, ILogger<StopHost> logger) : BaseEventHost(logger), IOrderedHook
{
    // This is in opposite order, so teardown happens in reverse when compared to start up
    private readonly IOnStop[] stop = stop.OrderBy(h => h.Order).ToArray();

    public int Order
    {
        get => int.MinValue;
    }

    public override int Count
    {
        get => stop.Length;
    }

    public override void Start()
    {
    }

    public override void Stop()
    {
        SafeEach(stop, h => h.OnStop());
    }
}
