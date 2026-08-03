using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.PluginStatus;

namespace Ocelot.Rotation.Services.BossMod;

public class BossModRotationProvider(IPluginStatus pluginStatus, IServiceProvider services) : IRotationProvider
{
    public const string Key = "BossMod";

    private const string BossModRebornInternalName = "BossModReborn";

    public string InternalName
    {
        get => Key;
    }

    public string DisplayName
    {
        get => "Boss Mod / BossMod Reborn";
    }

    public bool IsAvailable()
    {
        // Both plugins expose BossMod.Presets.* IPC (same prefix).
        return pluginStatus.IsLoaded(Key) || pluginStatus.IsLoaded(BossModRebornInternalName);
    }

    public IRotationService Create()
    {
        return services.GetRequiredService<BossModRotationService>();
    }
}
