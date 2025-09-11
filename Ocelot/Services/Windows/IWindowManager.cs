using System;
using Ocelot.Windows;

namespace Ocelot.Services.Windows;

public interface IWindowManager : IDisposable
{
    void Initialize();

    void ToggleConfigUI();

    void OpenConfigUI();

    void CloseConfigUI();

    bool IsConfigUIOpen();

    void ToggleMainUI();

    void OpenMainUI();

    void CloseMainUI();

    bool IsMainUIOpen();

    void AddWindow(OcelotWindow window);

    bool HasWindow<T>() where T : OcelotWindow;

    T GetWindow<T>() where T : OcelotWindow;

    bool TryGetWindow<T>(out T? window) where T : OcelotWindow;
}
