namespace Ocelot.Windows;

public abstract class OcelotConfigWindow(OcelotPlugin plugin, IOcelotConfig config) : OcelotWindow(plugin, config)
{
    public override void Render(RenderContext context)
    {
        plugin.Modules.RenderConfigUi(context);
    }

    protected override string GetWindowName()
    {
        return $"{I18N.T("windows.config.title")}##Config";
    }
}
