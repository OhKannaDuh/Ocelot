using Ocelot.Lifecycle;

namespace Ocelot.Rotation.Services;

public class DynamicRotationService(IEnumerable<IRotationProvider> providers) : IRotationService, IOnPreUpdate
{
    private string currentInternalName = "";

    private IRotationService? current;

    public void PreUpdate()
    {
        var bestMatch = providers.Where(p => p.IsAvailable()).OrderByDescending(p => p.Priority).FirstOrDefault();

        if (bestMatch?.InternalName == currentInternalName)
        {
            return;
        }

        current?.Unload();
        currentInternalName = bestMatch?.InternalName ?? "";
        current = bestMatch?.Create();
        current?.Load();
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
}
