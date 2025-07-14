using System.Linq;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using Ocelot.IPC;
using Ocelot.Windows;

namespace Ocelot.Modules;

public abstract class Module<P, C>(P plugin, C pluginConfig) : IModule
    where P : OcelotPlugin
    where C : IOcelotConfig
{
    public readonly P Plugin = plugin;

    public readonly C PluginConfig = pluginConfig;

    public virtual bool IsEnabled
    {
        get => true;
    }

    public virtual bool ShouldUpdate
    {
        get => IsEnabled;
    }

    public virtual bool ShouldRender
    {
        get => IsEnabled;
    }

    public virtual bool ShouldInitialize
    {
        get => IsEnabled;
    }

    public virtual ModuleConfig? Config
    {
        get => null;
    }

    public virtual void PreInitialize()
    {
    }

    public virtual void Initialize()
    {
    }

    public virtual void PostInitialize()
    {
    }

    public virtual void PreUpdate(UpdateContext context)
    {
    }

    public virtual void Update(UpdateContext context)
    {
    }

    public virtual void PostUpdate(UpdateContext context)
    {
    }

    public virtual void Render(RenderContext context)
    {
    }

    public virtual bool RenderMainUi(RenderContext context)
    {
        return false;
    }

    public virtual void RenderConfigUi(RenderContext context)
    {
        if (Config != null && Config.Draw())
        {
            PluginConfig.Save();
        }
    }

    public virtual void OnChatMessage(XivChatType type, int timestamp, SeString sender, SeString message, bool isHandled)
    {
    }

    public virtual void OnTerritoryChanged(ushort id)
    {
    }

    public virtual void Dispose()
    {
    }

    public void Debug(string log)
    {
        Svc.Log.Debug($"[{GetType().Name}] {log}");
    }

    public void Error(string log)
    {
        Svc.Log.Error($"[{GetType().Name}] {log}");
    }

    public void Fatal(string log)
    {
        Svc.Log.Fatal($"[{GetType().Name}] {log}");
    }

    public void Info(string log)
    {
        Svc.Log.Info($"[{GetType().Name}] {log}");
    }

    public void Verbose(string log)
    {
        Svc.Log.Verbose($"[{GetType().Name}] {log}");
    }

    public void Warning(string log)
    {
        Svc.Log.Warning($"[{GetType().Name}] {log}");
    }

    public string Translate(string key)
    {
        string ToSnakeCase(string input)
        {
            return string.Concat(input.Select((x, i) =>
                i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
        }

        var module = ToSnakeCase(GetType().Name).Replace("_module", "");
        key = $"modules.{module}." + key;

        return I18N.T(key);
    }

    public string Trans(string key)
    {
        return Translate(key);
    }

    public string T(string key)
    {
        return Translate(key);
    }

    // Accessors for OcelotPlugin managers
    // Modules
    public ModuleManager modules
    {
        get => Plugin.Modules;
    }

    public T GetModule<T>() where T : class, IModule
    {
        return modules.GetModule<T>();
    }


    public bool TryGetModule<T>(out T? module) where T : class, IModule
    {
        return modules.TryGetModule(out module);
    }

    // IPC
    public IPCManager ipc
    {
        get => Plugin.IPC;
    }

    public T GetIPCProvider<T>() where T : IPCSubscriber
    {
        return ipc.GetProvider<T>();
    }

    public bool TryGetIPCProvider<T>(out T? provider) where T : IPCSubscriber
    {
        return ipc.TryGetProvider(out provider);
    }

    // Windows
    public WindowManager windows
    {
        get => Plugin.Windows;
    }

    public T GetWindow<T>() where T : OcelotWindow
    {
        return windows.GetWindow<T>();
    }

    public bool TryGetWindow<T>(out T? provider) where T : OcelotWindow
    {
        return windows.TryGetWindow(out provider);
    }
}
