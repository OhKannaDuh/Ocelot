using System;
using Dalamud.Interface.Windowing;
using Ocelot.Services;
using Ocelot.Services.Translation;
using Ocelot.Services.Windows;

namespace Ocelot.Windows;

public abstract class OcelotWindow : Window, IDisposable
{
    protected static ITranslationService Translator {
        get => OcelotServices.GetCached<ITranslationService>();
    }

    public static IWindowManager WindowManager {
        get => OcelotServices.GetCached<IWindowManager>();
    }

    protected readonly OcelotPlugin Plugin;

    protected readonly OcelotConfig Config;

    protected OcelotWindow(OcelotPlugin plugin, OcelotConfig config)
        : base("")
    {
        Plugin = plugin;
        Config = config;

        WindowName = GetWindowName();
        Translator.LanguageChanged += (oldLang, newLang) => { WindowName = GetWindowName(); };
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

    // public virtual void PreInitialize() { }
    //
    // public virtual void Initialize() { }
    //
    // public virtual void PostInitialize() { }

    public virtual void Dispose() { }

    protected abstract string GetWindowName();
}
