using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.PluginStatus;

namespace Ocelot.Rotation.Services.Wrath;

public class WrathRotationProvider(IPluginStatus pluginStatus, IServiceProvider services) : IRotationProvider
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
        return pluginStatus.IsLoaded(InternalName);
    }

    public IRotationService Create()
    {
        return services.GetRequiredService<WrathRotationService>();
    }
}
