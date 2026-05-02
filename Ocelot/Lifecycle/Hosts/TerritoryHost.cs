using Dalamud.Plugin.Services;
using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public class TerritoryHost(
    IEnumerable<IOnTerritoryChanged> territoryChanged,
    IClientState clientState,
    ILogger<TerritoryHost> logger
) : BaseEventHost(logger)
{
    private readonly IOnTerritoryChanged[] territoryChanged = territoryChanged.OrderByDescending(h => h.Order).ToArray();

    public override int Count
    {
        get => territoryChanged.Length;
    }

    public override void Start()
    {
        if (Count == 0)
        {
            return;
        }

        clientState.TerritoryChanged += TerritoryChanged;
    }

    public override void Stop()
    {
        if (Count == 0)
        {
            return;
        }

        clientState.TerritoryChanged -= TerritoryChanged;
    }

    private void TerritoryChanged(ushort territory)
    {
        SafeEach(territoryChanged, h => h.OnTerritoryChanged(territory));
    }
}
