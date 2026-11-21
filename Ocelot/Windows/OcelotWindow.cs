using Dalamud.Interface.Windowing;
using Ocelot.UI;

namespace Ocelot.Windows;

public abstract class OcelotWindow(string name) : Window(name), IWindow
{
    private bool themePushed;

    protected abstract void Render();

    public override void PreDraw()
    {
        OcelotTheme.Push();
        themePushed = true;
    }

    public override void PostDraw()
    {
        if (themePushed)
        {
            themePushed = false;
            OcelotTheme.Pop();
        }
    }

    public override void Draw()
    {
        Render();
    }
}
