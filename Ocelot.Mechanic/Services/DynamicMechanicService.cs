using Ocelot.Lifecycle;
using Ocelot.Services.Logger;

namespace Ocelot.Mechanic.Services;

public class DynamicMechanicService(
    IEnumerable<IMechanicProvider> providers,
    IMechanicPriorityService priority,
    ILogger<DynamicMechanicService> logger
) : IMechanicService, IOnPreUpdate
{
    private string currentInternalName = "";

    private IMechanicService? current;

    public void PreUpdate()
    {
        var order = priority.GetPriority().ToList();
        var rank = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < order.Count; i++)
        {
            rank[order[i]] = i;
        }

        var bestMatch = providers
            .Where(p => p.IsAvailable())
            .OrderBy(p => rank.GetValueOrDefault(p.InternalName, int.MaxValue))
            .ThenBy(p => p.InternalName, StringComparer.Ordinal)
            .FirstOrDefault();

        if ((bestMatch?.InternalName ?? "") == currentInternalName)
        {
            return;
        }

        currentInternalName = bestMatch?.InternalName ?? "";
        current = bestMatch?.Create();

        logger.Info("Dynamic mechanic service has been updated to {name}", currentInternalName);
    }

    public void Enable()
    {
        current?.Enable();
    }

    public void Disable()
    {
        current?.Disable();
    }
}
