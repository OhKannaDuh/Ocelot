namespace Ocelot.Windows;

public abstract class OcelotConfigWindow : OcelotWindow
{
    public OcelotConfigWindow(OcelotPlugin plugin, IOcelotConfig config)
        : base(plugin, config, $"{I18N.T("windows.config.title")}##Config") { }

    public override void Draw() => plugin.modules.DrawConfigUi();
}
