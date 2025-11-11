using Ocelot.Services.Translation;
using Ocelot.Services.WindowManager;

namespace Ocelot.Windows;

public sealed class MainWindow : OcelotWindow, IMainWindow, IDisposable
{
    private readonly IMainRenderer renderer;

    private readonly ITranslator translator;

    public MainWindow(IMainRenderer renderer, ITranslator<MainWindow> translator)
        : base(translator.T("windows.main.title"))
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
        WindowName = translator.T(".title");
    }

    public void Dispose()
    {
        translator.TranslationsChanged -= UpdateWindowTitle;
        translator.LanguageChanged -= UpdateWindowTitle;
    }
}
