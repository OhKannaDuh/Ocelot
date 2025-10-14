using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin.Services;

namespace Ocelot;
#nullable disable

public sealed class DalamudServices
{
    [PluginService] public IFramework Framework { get; private set; }

    [PluginService] public IDataManager DataManager { get; private set; }

    [PluginService] public IClientState ClientState { get; private set; }

    [PluginService] public ITargetManager TargetManager { get; private set; }

    [PluginService] public ITextureProvider TextureProvider { get; private set; }
}
