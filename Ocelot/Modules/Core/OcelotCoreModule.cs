namespace Ocelot.Modules;

[OcelotModule(ConfigGroup = "ocelot")]
public class OcelotCoreModule(OcelotPlugin plugin, OcelotConfig config) : Module<OcelotPlugin, OcelotConfig>(plugin, config)
{
    public override OcelotCoreConfig Config
    {
        get => PluginConfig.OcelotCoreConfig;
    }
}
