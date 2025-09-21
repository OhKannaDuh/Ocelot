using Dalamud.Interface.Windowing;

namespace Ocelot.Windows;

public abstract class OcelotWindow(string name) : Window(name), IWindow
{
    protected abstract void Render();

    public override void Draw()
    {
        Render();
    }
}
