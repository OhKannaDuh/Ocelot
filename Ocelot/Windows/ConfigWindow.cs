using Ocelot.Services.Translation;
using Ocelot.Services.WindowManager;

namespace Ocelot.Windows;

public sealed class ConfigWindow : OcelotWindow, IConfigWindow, IDisposable
{
    private readonly IConfigRenderer renderer;

    private readonly ITranslator translator;

    public ConfigWindow(IConfigRenderer renderer, ITranslator<ConfigWindow> translator)
        : base(translator.T("windows.config.title") + "###ocelot.config")
    {
        this.renderer = renderer;
        this.translator = translator;

        translator.LanguageChanged += UpdateWindowTitle;
        translator.TranslationsChanged += UpdateWindowTitle;
    }

    protected override void Render()
    {
        renderer.Render();
    }

    private void UpdateWindowTitle()
    {
        WindowName = translator.T(".title") + "###ocelot.config";
    }

    public void Dispose()
    {
        translator.TranslationsChanged -= UpdateWindowTitle;
        translator.LanguageChanged -= UpdateWindowTitle;
    }
}
