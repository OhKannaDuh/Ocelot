using System;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Ocelot.IPC;
using Ocelot.Windows;

namespace Ocelot.Modules;

public interface IModule : IDisposable
{
    bool enabled { get; }

    bool tick { get; }

    bool render { get; }

    ModuleConfig? config { get; }

    void PreInitialize();

    void Initialize();

    void PostInitialize();

    // Functions
    void Tick(IFramework framework);

    void Draw();

    bool DrawMainUi();

    void DrawConfigUi();

    // Events
    void OnChatMessage(XivChatType type, int timestamp, SeString sender, SeString message, bool isHandled);

    void OnTerritoryChanged(ushort id);

    void Debug(string log);

    void Error(string log);

    void Fatal(string log);

    void Info(string log);

    void Verbose(string log);

    void Warning(string log);

    string Translate(string key);

    string Trans(string key);

    string T(string key);

    T GetModule<T>() where T : class, IModule;

    bool TryGetModule<T>(out T? module) where T : class, IModule;

    T GetIPCProvider<T>() where T : IPCProvider;

    bool TryGetIPCProvider<T>(out T? provider) where T : IPCProvider;

    T GetWindow<T>() where T : OcelotWindow;

    bool TryGetWindow<T>(out T? provider) where T : OcelotWindow;
}
