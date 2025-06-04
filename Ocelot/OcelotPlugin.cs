using Dalamud.Plugin;

namespace Ocelot;

public abstract class OcelotPlugin : IDalamudPlugin
{
    public abstract IOcelotConfig _config { get; }

    public OcelotPlugin() { }

    public void Dispose() { }
}
