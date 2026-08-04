using Ocelot.Lifecycle;
using Ocelot.Services.Logger;

namespace Ocelot.Rotation.Services;

public class DynamicRotationService(
    IEnumerable<IRotationProvider> providers,
    IRotationPriorityService priority,
    ILogger<DynamicRotationService> logger
) : IRotationService, IOnPreUpdate
{
    private string currentInternalName = "";

    private IRotationService? current;

    public void PreUpdate()
    {
        var order = priority.GetPriority().ToList();
        var rank = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < order.Count; i++)
        {
            rank[order[i]] = i;
        }

        var bestMatch = providers
            .Where(p => p.IsAvailable() && rank.ContainsKey(p.InternalName))
            .OrderBy(p => rank[p.InternalName])
            .ThenBy(p => p.InternalName, StringComparer.Ordinal)
            .FirstOrDefault();

        if ((bestMatch?.InternalName ?? "") == currentInternalName)
        {
            // BossMod/BMR IPC may not be ready on the first Load after plugin start.
            current?.Refresh();
            return;
        }

        current?.Unload();
        currentInternalName = bestMatch?.InternalName ?? "";
        current = bestMatch?.Create();
        current?.Load();

        logger.Info("Dynamic rotation service has been updated to {name}", currentInternalName);
    }

    public void Load()
    {
        current?.Load();
    }

    public void Unload()
    {
        current?.Unload();
    }

    public void EnableAutoRotation()
    {
        current?.EnableAutoRotation();
    }

    public void DisableAutoRotation()
    {
        current?.DisableAutoRotation();
    }

    public void EnableSingleTarget()
    {
        current?.EnableSingleTarget();
    }

    public void DisableSingleTarget()
    {
        current?.DisableSingleTarget();
    }

    public void EnsureAutoRotationPreset()
    {
        current?.EnsureAutoRotationPreset();
    }

    public void DestroyAutoRotationPreset()
    {
        current?.DestroyAutoRotationPreset();
    }
}
