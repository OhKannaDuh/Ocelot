using Ocelot.Modules;

namespace Ocelot;

public abstract class OcelotConfig : IOcelotConfig
{
    public abstract int Version { get; set; }

    public OcelotCoreConfig OcelotCoreConfig { get; set; } = new();

    public abstract void Save();
}
