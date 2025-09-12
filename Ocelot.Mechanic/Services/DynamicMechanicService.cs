using Ocelot.Lifecycle;

namespace Ocelot.Mechanic.Services;

public class DynamicMechanicService(IEnumerable<IMechanicProvider> providers) : IMechanicService, IOnPreUpdate
{
    private string currentInternalName = "";

    private IMechanicService? current;

    public void PreUpdate()
    {
        var bestMatch = providers.Where(p => p.IsAvailable()).OrderByDescending(p => p.Priority).FirstOrDefault();

        if (bestMatch?.InternalName == currentInternalName)
        {
            return;
        }

        currentInternalName = bestMatch?.InternalName ?? "";
        current = bestMatch?.Create();
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
