using Ocelot.Intents;
using Ocelot.UI;
using Ocelot.Windows;

namespace Ocelot.Modules;

[OcelotModule(ConfigGroup = "ocelot")]
public class OcelotCoreModule(OcelotPlugin plugin, OcelotConfig config) : Module<OcelotPlugin, OcelotConfig>(plugin, config), IMainRenderable
{
    public override OcelotCoreConfig Config {
        get => PluginConfig.OcelotCoreConfig;
    }

    public bool RenderMainUi(RenderContext ctx)
    {
        OcelotUI.LabelledValue("Ocelot Version", OcelotPlugin.OcelotVersion);
        return true;
    }
}
