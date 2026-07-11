using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public class StopHost(IServiceProvider services, ILogger<StopHost> logger) : BaseEventHost(logger), IOrderedHook
{
    // This is in opposite order, so teardown happens in reverse when compared to start up
    private IOnStop[]? stopHooks;

    private IOnStop[] StopHooks =>
        stopHooks ??= services.GetServices<IOnStop>().OrderBy(h => h.Order).ToArray();

    public int Order
    {
        get => int.MinValue;
    }

    public override int Count
    {
        get => stopHooks?.Length ?? 0;
    }

    public override void Start()
    {
    }

    public override void Stop()
    {
        SafeEach(StopHooks, h => h.OnStop());
    }
}
