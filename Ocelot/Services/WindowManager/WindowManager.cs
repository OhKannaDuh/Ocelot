using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Ocelot.Lifecycle;
using Ocelot.Services.Logger;
using Ocelot.Windows;

namespace Ocelot.Services.WindowManager;

public class WindowManager : IWindowManager, IOnRender, IDisposable
{
    private readonly IDalamudPluginInterface plugin;

    private readonly WindowSystem manager;

    private readonly IMainWindow? mainWindow;

    private readonly IConfigWindow? configWindow;

    public WindowManager(
        IDalamudPluginInterface plugin,
        IEnumerable<IWindow> windows,
        ILogger<WindowManager> logger,
        IMainWindow? mainWindow = null,
        IConfigWindow? configWindow = null)
    {
        this.plugin = plugin;
        manager = new WindowSystem($"OcelotWindowManager##{plugin.InternalName}");
        this.mainWindow = mainWindow;
        this.configWindow = configWindow;

        foreach (var window in windows.OfType<Window>())
        {
            logger.Info("Adding window, {name}", window.WindowName);
            manager.AddWindow(window);
        }

        if (mainWindow is Window main)
        {
            manager.AddWindow(main);
        }

        if (configWindow is Window config)
        {
            manager.AddWindow(config);
        }

        plugin.UiBuilder.OpenMainUi += ToggleMain;
        plugin.UiBuilder.OpenConfigUi += ToggleConfig;
    }

    private void ToggleMain()
    {
        mainWindow?.Toggle();
    }

    private void ToggleConfig()
    {
        configWindow?.Toggle();
    }

    public void Render()
    {
        manager.Draw();
    }

    public void Dispose()
    {
        manager.RemoveAllWindows();

        plugin.UiBuilder.OpenMainUi -= ToggleMain;
        plugin.UiBuilder.OpenConfigUi -= ToggleConfig;
    }
}
