using Dalamud.IoC;
using Dalamud.Plugin.Services;

namespace Ocelot;
#nullable disable

public sealed class DalamudServices
{
    [PluginService] public IFramework Framework { get; private set; }

    [PluginService] public IDataManager Data { get; private set; }
}
