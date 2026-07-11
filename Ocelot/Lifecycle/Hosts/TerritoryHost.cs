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
    private IOnTerritoryChanged[]? territoryHooks;

    private IOnTerritoryChanged[] TerritoryHooks =>
        territoryHooks ??= services.GetServices<IOnTerritoryChanged>().OrderByDescending(h => h.Order).ToArray();

    public override int Count
    {
        get => territoryHooks?.Length ?? 0;
    }

    public override void Start()
    {
        if (TerritoryHooks.Length == 0)
        {
            return;
        }

        clientState.TerritoryChanged += TerritoryChanged;
    }

    public override void Stop()
    {
        if (territoryHooks == null || territoryHooks.Length == 0)
        {
            return;
        }

        clientState.TerritoryChanged -= TerritoryChanged;
    }

    private void TerritoryChanged(uint territory)
    {
        SafeEach(TerritoryHooks, h => h.OnTerritoryChanged(territory));
    }
}
