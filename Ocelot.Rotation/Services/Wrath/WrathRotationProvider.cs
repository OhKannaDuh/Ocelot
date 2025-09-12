using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.PluginStatus;

namespace Ocelot.Rotation.Services.Wrath;

public class WrathRotationProvider(IPluginStatusService pluginStatus, IServiceProvider services) : IRotationProvider
{
    public string InternalName {
        get => "WrathCombo";
    }

    public string DisplayName {
        get => "Wrath Combo";
    }

    public int Priority {
        get => RotationPriority.WrathCombo;
    }

    public bool IsAvailable()
    {
        return pluginStatus.IsLoaded(InternalName);
    }

    public IRotationService Create()
    {
        return services.GetRequiredService<WrathRotationService>();
    }
}
