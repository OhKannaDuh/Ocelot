using System;
using Dalamud.Interface.Windowing;

namespace Ocelot.Windows;

public abstract class OcelotWindow : Window, IDisposable
{
    protected readonly OcelotPlugin plugin;

    protected readonly IOcelotConfig config;

    public OcelotWindow(OcelotPlugin plugin, IOcelotConfig config, string name)
        : base(name)
    {
        this.plugin = plugin;
        this.config = config;
    }

    public virtual void Dispose() { }
}
