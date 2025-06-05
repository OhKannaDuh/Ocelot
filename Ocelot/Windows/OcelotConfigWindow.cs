namespace Ocelot.Windows;

public abstract class OcelotConfigWindow : OcelotWindow
{

    public OcelotConfigWindow(OcelotPlugin plugin, IOcelotConfig config)
        : base(plugin, config, $"{plugin.Name}##Config")
    {
    }

    public override void Draw() => plugin.modules.DrawConfigUi();
}
