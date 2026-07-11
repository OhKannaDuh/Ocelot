using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public class StartHost(IServiceProvider services, ILogger<StartHost> logger) : BaseEventHost(logger), IOrderedHook
{
    private IOnStart[]? startHooks;

    private IOnStart[] StartHooks =>
        startHooks ??= services.GetServices<IOnStart>().OrderByDescending(h => h.Order).ToArray();

    public int Order
    {
        get => int.MaxValue;
    }

    public override int Count
    {
        get => startHooks?.Length ?? 0;
    }

    public override void Start()
    {
        SafeEach(StartHooks, h => h.OnStart());
    }

    public override void Stop()
    {
    }
}
