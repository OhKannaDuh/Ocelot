using System;
using Dalamud.Interface.Windowing;

namespace Ocelot.Windows;

public abstract class OcelotWindow : Window, IDisposable
{
    protected readonly OcelotPlugin plugin;

    protected readonly IOcelotConfig config;

    public OcelotWindow(OcelotPlugin plugin, IOcelotConfig config)
        : base("")
    {
        this.plugin = plugin;
        this.config = config;

        WindowName = GetWindowName();
        I18N.OnLanguageChanged += (oldLang, newLang) => { WindowName = GetWindowName(); };
    }

    protected abstract void Render(RenderContext context);

    public override void Draw()
    {
        if (plugin.RenderContext == null)
        {
            return;
        }

        Render(plugin.RenderContext);
    }

    public virtual void PreInitialize()
    {
    }

    public virtual void Initialize()
    {
    }

    public virtual void PostInitialize()
    {
    }

    public virtual void Dispose()
    {
    }

    protected abstract string GetWindowName();
}
