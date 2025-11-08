using Ocelot.Services.Translation;
using Ocelot.Services.WindowManager;

namespace Ocelot.Windows;

public sealed class ConfigWindow : OcelotWindow, IConfigWindow, IDisposable
{
    private readonly IConfigRenderer renderer;

    private readonly ITranslator translator;

    public ConfigWindow(IConfigRenderer renderer, ITranslator translator)
        : base(translator.T("windows.config.title"))
    {
        this.renderer = renderer;
        this.translator = translator.WithScope("windows.config");

        translator.LanguageChanged += UpdateWindowTitle;
        translator.TranslationsChanged += UpdateWindowTitle;
    }

    protected override void Render()
    {
        renderer.Render();
    }

    private void UpdateWindowTitle()
    {
        WindowName = translator.T(".title");
    }

    public void Dispose()
    {
        translator.TranslationsChanged -= UpdateWindowTitle;
        translator.LanguageChanged -= UpdateWindowTitle;
    }
}
