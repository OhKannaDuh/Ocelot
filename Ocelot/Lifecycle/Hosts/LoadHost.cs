using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public class LoadHost(IServiceProvider services, ILogger<LoadHost> logger) : BaseEventHost(logger), IOrderedHook
{
    private readonly Lazy<IOnLoad[]> loadHooks = new(() =>
        services.GetServices<IOnLoad>().OrderByDescending(h => h.Order).ToArray());

    public int Order
    {
        get => int.MaxValue;
    }

    public override int Count
    {
        get => loadHooks.IsValueCreated ? loadHooks.Value.Length : 0;
    }

    public override void Start()
    {
        SafeEach(loadHooks.Value, h => h.OnLoad());
    }

    public override void Stop()
    {
    }
}
