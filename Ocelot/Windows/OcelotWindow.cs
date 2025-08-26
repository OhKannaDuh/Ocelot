using System;
using Dalamud.Interface.Windowing;

namespace Ocelot.Windows;

public abstract class OcelotWindow : Window, IDisposable
{
    protected readonly OcelotPlugin Plugin;

    protected readonly OcelotConfig PluginConfig;

    protected OcelotWindow(OcelotPlugin plugin, OcelotConfig pluginConfig)
        : base("")
    {
        Plugin = plugin;
        PluginConfig = pluginConfig;

        WindowName = GetWindowName();
        I18N.OnLanguageChanged += (oldLang, newLang) => { WindowName = GetWindowName(); };
    }

    protected abstract void Render(RenderContext context);

    public override void Draw()
    {
        if (Plugin.RenderContext == null)
        {
            return;
        }

        Render(Plugin.RenderContext);
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
