using Dalamud.Interface.Windowing;

namespace Ocelot.Windows;

/// <summary>
///     Lets plugins add title-bar buttons to <see cref="MainWindow"/> (e.g. illegal mode).
/// </summary>
public interface IMainWindowTitleBarContributor
{
    void Contribute(ICollection<TitleBarButton> buttons);
}
