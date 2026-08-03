using Microsoft.Extensions.DependencyInjection;

namespace Ocelot.Rotation.Services.Wrath;

public class WrathRotationProvider(IServiceProvider services) : IRotationProvider
{
    public const string Key = "WrathCombo";

    public string InternalName
    {
        get => Key;
    }

    public string DisplayName
    {
        get => "Wrath Combo";
    }

    public bool IsAvailable()
    {
        // Disabled — Wrath IPC leases lock the user's auto-rotation UI.
        return false;
    }

    public IRotationService Create()
    {
        return services.GetRequiredService<WrathRotationService>();
    }
}
