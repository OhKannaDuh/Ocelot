using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public class StartHost(IServiceProvider services, ILogger<StartHost> logger) : BaseEventHost(logger), IOrderedHook
{
    private readonly Lazy<IOnStart[]> startHooks = new(() =>
        services.GetServices<IOnStart>().OrderByDescending(h => h.Order).ToArray());

    public int Order
    {
        get => int.MaxValue;
    }

    public override int Count
    {
        get => startHooks.IsValueCreated ? startHooks.Value.Length : 0;
    }

    public override void Start()
    {
        SafeEach(startHooks.Value, h => h.OnStart());
    }

    public override void Stop()
    {
    }
}
