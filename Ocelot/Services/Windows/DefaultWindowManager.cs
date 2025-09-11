using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using Ocelot.Services.Logger;
using Ocelot.Windows;

namespace Ocelot.Services.Windows;

[OcelotService(typeof(IWindowManager))]
public class DefaultWindowManager : IWindowManager
{
    private readonly WindowSystem manager = new($"Ocelot##{Svc.PluginInterface?.InternalName}");

    private static OcelotMainWindow? MainWindow {
        get => OcelotServices.GetOptional<OcelotMainWindow>();
    }

    private static OcelotConfigWindow? ConfigWindow {
        get => OcelotServices.GetOptional<OcelotConfigWindow>();
    }

    private static List<OcelotWindow> Windows {
        get => OcelotServices.Container.GetAll<OcelotWindow>().ToList();
    }

    private static ILoggerService Logger {
        get => OcelotServices.GetCached<ILoggerService>();
    }

    public void Initialize()
    {
        foreach (var window in Windows)
        {
            manager.AddWindow(window);
        }

        Svc.PluginInterface.UiBuilder.Draw += manager.Draw;
        Svc.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        Svc.PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    }

    public void ToggleConfigUI()
    {
        ConfigWindow?.Toggle();
    }

    public void OpenConfigUI()
    {
        if (!IsConfigUIOpen())
        {
            ToggleConfigUI();
        }
    }

    public void CloseConfigUI()
    {
        if (IsConfigUIOpen())
        {
            ToggleConfigUI();
        }
    }

    public bool IsConfigUIOpen()
    {
        return ConfigWindow?.IsOpen ?? false;
    }

    public void ToggleMainUI()
    {
        MainWindow?.Toggle();
    }

    public void OpenMainUI()
    {
        if (!IsMainUIOpen())
        {
            ToggleMainUI();
        }
    }

    public void CloseMainUI()
    {
        if (IsMainUIOpen())
        {
            ToggleMainUI();
        }
    }

    public bool IsMainUIOpen()
    {
        return MainWindow?.IsOpen ?? false;
    }

    public void AddWindow(OcelotWindow window)
    {
        if (!Windows.Contains(window))
        {
            manager.AddWindow(window);
            Windows.Add(window);
        }
    }

    public bool HasWindow<T>() where T : OcelotWindow
    {
        return Windows.OfType<T>().Any();
    }

    public T GetWindow<T>() where T : OcelotWindow
    {
        var window = Windows.OfType<T>().FirstOrDefault();
        if (window == null)
        {
            throw new UnableToLoadWindowException($"Window of type {typeof(T).Name} was not found.");
        }

        return window;
    }

    public bool TryGetWindow<T>(out T? window) where T : OcelotWindow
    {
        try
        {
            window = GetWindow<T>();
            return true;
        }
        catch (UnableToLoadWindowException ex)
        {
            Logger.Error(ex.Message);
            window = null;
            return false;
        }
    }

    public void Dispose()
    {
        Svc.PluginInterface.UiBuilder.Draw -= manager.Draw;
        Svc.PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUI;
        Svc.PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUI;

        manager.RemoveAllWindows();

        foreach (var window in Windows)
        {
            window.Dispose();
        }

        Windows.Clear();
    }
}
