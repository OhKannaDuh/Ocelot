namespace Ocelot.Windows;

public interface IWindow
{
    string WindowName { get; }

    bool IsOpen { get; set; }

    void Toggle();
}
