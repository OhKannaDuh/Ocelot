using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.Translation;
using Ocelot.Services.Translation.Extensions;
using Ocelot.Services.WindowManager;

namespace Ocelot.Windows;

public sealed class MainWindow : OcelotWindow, IMainWindow, IDisposable
{
    private readonly IMainRenderer renderer;

    private readonly ITranslator translator;

    private readonly IConfigWindow configWindow;

    public MainWindow(
        IMainRenderer renderer,
        ITranslator<MainWindow> translator,
        IConfigWindow configWindow,
        IEnumerable<IMainWindowTitleBarContributor> titleBarContributors)
        : base(translator.T("windows.main.title") + "###ocelot.main")
    {
        this.renderer = renderer;
        this.translator = translator;
        this.configWindow = configWindow;

        TitleBarButtons.Add(new TitleBarButton
        {
            Click = m =>
            {
                if (m != ImGuiMouseButton.Left)
                {
                    return;
                }

                configWindow.Toggle();
            },
            Icon = FontAwesomeIcon.Cog,
            IconOffset = new Vector2(2, 2),
            ShowTooltip = () => ImGui.SetTooltip(translator.ToggleConfigWindow()),
        });

        foreach (IMainWindowTitleBarContributor contributor in titleBarContributors)
        {
            contributor.Contribute(TitleBarButtons);
        }

        translator.LanguageChanged += UpdateWindowTitle;
        translator.TranslationsChanged += UpdateWindowTitle;
    }

    protected override void Render()
    {
        renderer.Render();
    }

    private void UpdateWindowTitle()
    {
        WindowName = translator.T(".title") + "###ocelot.main";
    }

    public void Dispose()
    {
        translator.TranslationsChanged -= UpdateWindowTitle;
        translator.LanguageChanged -= UpdateWindowTitle;
    }
}
