using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using Ocelot.IPC;

namespace Ocelot.Modules;

public abstract class Module<P, C> : IModule
    where P : OcelotPlugin
    where C : IOcelotConfig
{
    public readonly P plugin;

    public readonly C _config;

    // Accessors for OcelotPlugin managers
    // Modules
    public ModuleManager modules => plugin.modules;

    public T? GetModule<T>() where T : class, IModule => modules.GetModule<T>();


    public bool TryGetModule<T>(out T? module) where T : class, IModule => modules.TryGetModule(out module);

    // IPC
    public IPCManager ipc => plugin.ipc;

    public T? GetIPCProvider<T>() where T : IPCProvider => ipc.GetProvider<T>();

    public bool TryGetIPCProvider<T>(out T? provider) where T : IPCProvider => ipc.TryGetProvider(out provider);

    public virtual bool enabled => true;

    public virtual ModuleConfig? config
    {
        get => null;
    }

    public Module(P plugin, C config)
    {
        this.plugin = plugin;
        this._config = config;
    }

    public virtual void PreInitialize() { }

    public virtual void Initialize() { }

    public virtual void PostInitialize() { }

    public virtual void Tick(IFramework _) { }

    public virtual void Draw() { }

    public virtual bool DrawMainUi() => false;

    public virtual void DrawConfigUi()
    {
        if (config != null && config.Draw())
        {
            _config.Save();
        }
    }

    public virtual void OnChatMessage(XivChatType type, int timestamp, SeString sender, SeString message, bool isHandled) { }

    public virtual void OnTerritoryChanged(ushort id) { }

    public virtual void Dispose() { }

    public void Debug(string log) => Svc.Log.Debug($"[{GetType().Name}] {log}");

    public void Error(string log) => Svc.Log.Error($"[{GetType().Name}] {log}");

    public void Fatal(string log) => Svc.Log.Fatal($"[{GetType().Name}] {log}");

    public void Info(string log) => Svc.Log.Info($"[{GetType().Name}] {log}");

    public void Verbose(string log) => Svc.Log.Verbose($"[{GetType().Name}] {log}");

    public void Warning(string log) => Svc.Log.Warning($"[{GetType().Name}] {log}");
}
