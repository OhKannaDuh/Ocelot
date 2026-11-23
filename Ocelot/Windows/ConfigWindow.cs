using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
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

        Size = new Vector2(760, 620);
        SizeCondition = ImGuiCond.FirstUseEver;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(520, 360),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };

        translator.LanguageChanged += UpdateWindowTitle;
        translator.TranslationsChanged += UpdateWindowTitle;
    }

    protected override void Render()
    {
        renderer.Render();
    }

    public void OpenGroup(string groupKey)
    {
        renderer.SelectGroup(groupKey);
        IsOpen = true;
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
