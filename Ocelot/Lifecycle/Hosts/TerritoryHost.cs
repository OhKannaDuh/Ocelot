using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public class TerritoryHost(
    IServiceProvider services,
    IClientState clientState,
    ILogger<TerritoryHost> logger
) : BaseEventHost(logger)
{
    private readonly Lazy<IOnTerritoryChanged[]> territoryHooks = new(() =>
        services.GetServices<IOnTerritoryChanged>().OrderByDescending(h => h.Order).ToArray());

    public override int Count
    {
        get => territoryHooks.IsValueCreated ? territoryHooks.Value.Length : 0;
    }

    public override void Start()
    {
        if (territoryHooks.Value.Length == 0)
        {
            return;
        }

        clientState.TerritoryChanged += TerritoryChanged;
    }

    public override void Stop()
    {
        if (!territoryHooks.IsValueCreated || territoryHooks.Value.Length == 0)
        {
            return;
        }

        clientState.TerritoryChanged -= TerritoryChanged;
    }

    private void TerritoryChanged(uint territory)
    {
        SafeEach(territoryHooks.Value, h => h.OnTerritoryChanged(territory));
    }
}
