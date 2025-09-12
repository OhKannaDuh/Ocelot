using Dalamud.IoC;
using Dalamud.Plugin.Services;

namespace Ocelot;
#nullable disable

public sealed class DalamudServices
{
    [PluginService]
    public IFramework Framework { get; private set; }
}
