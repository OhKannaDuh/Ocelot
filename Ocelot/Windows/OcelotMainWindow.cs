namespace Ocelot.Windows;

public abstract class OcelotMainWindow : OcelotWindow
{
    public OcelotMainWindow(OcelotPlugin plugin, IOcelotConfig config)
        : base(plugin, config, $"{plugin.Name}##Main")
    {
    }
}
