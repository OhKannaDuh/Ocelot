using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Interface.Windowing;
using ECommons;
using ECommons.DalamudServices;
using Ocelot.Modules;

namespace Ocelot.Windows;

public class WindowManager : IDisposable
{
    private readonly WindowSystem windows = new($"Ocelot##{Svc.PluginInterface?.InternalName}");

    private OcelotMainWindow? mainWindow;

    private OcelotConfigWindow? configWindow;

    public void Initialize(OcelotPlugin plugin, IOcelotConfig config)
    {
        if (Registry.GetTypesWithAttribute<OcelotMainWindowAttribute>().TryGetFirst(out var mainWindowType))
        {
            mainWindow = Activator.CreateInstance(mainWindowType, plugin, config) as OcelotMainWindow;
            if (mainWindow != null)
            {
                windows.AddWindow(mainWindow);
            }
        }

        if (Registry.GetTypesWithAttribute<OcelotConfigWindowAttribute>().TryGetFirst(out var configWindowType))
        {
            configWindow = Activator.CreateInstance(configWindowType, plugin, config) as OcelotConfigWindow;
            if (configWindow != null)
            {
                windows.AddWindow(configWindow);
            }
        }

        Svc.PluginInterface.UiBuilder.Draw += windows.Draw;
        Svc.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        Svc.PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    }

    public void ToggleConfigUI() => configWindow?.Toggle();

    public void ToggleMainUI() => mainWindow?.Toggle();

    public void Dispose()
    {
        Svc.PluginInterface.UiBuilder.Draw -= windows.Draw;
        Svc.PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUI;
        Svc.PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUI;

        windows.RemoveAllWindows();

        configWindow?.Dispose();
        mainWindow?.Dispose();
    }
}
