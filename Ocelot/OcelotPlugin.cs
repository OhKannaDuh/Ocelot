using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.DalamudServices;
using Ocelot.Commands;
using Ocelot.IPC;
using Ocelot.Modules;
using Ocelot.Windows;
using Pictomancy;

namespace Ocelot;

public abstract class OcelotPlugin : IDalamudPlugin
{
    public abstract string Name { get; }

    public abstract IOcelotConfig _config { get; }

    public readonly ModuleManager modules = new();

    public readonly WindowManager windows = new();

    public readonly CommandManager commands = new();

    public readonly IPCManager ipc = new();

    private List<OcelotFeature> features = [];

    public OcelotPlugin(IDalamudPluginInterface plugin, params Module[] eModules)
    {
        ECommonsMain.Init(plugin, this, eModules);
        PictoService.Initialize(plugin);

        Registry.RegisterAssemblies(typeof(OcelotPlugin).Assembly);
        Registry.RegisterAssemblies(GetType().Assembly);
    }

    protected void OcelotInitialize(params OcelotFeature[] features)
    {
        if (features.Length <= 0)
        {
            this.features.Add(OcelotFeature.All);
        }
        else
        {
            this.features = features.ToList();
        }

        if (this.features.ContainsAny(OcelotFeature.ModuleManager, OcelotFeature.All))
        {
            Logger.Info("Initializing Module Manager...");
            modules.AutoRegister(this, _config);
            modules.PreInitialize();
            modules.Initialize();
            Svc.PluginInterface.UiBuilder.Draw += modules.Draw;
        }

        if (this.features.ContainsAny(OcelotFeature.WindowManager, OcelotFeature.All))
        {
            Logger.Info("Initializing Window Manager...");
            windows.Initialize(this, _config);
        }

        if (this.features.ContainsAny(OcelotFeature.CommandManager, OcelotFeature.All))
        {
            Logger.Info("Initializing Command Manager...");
            commands.Initialize(this);
        }

        if (this.features.ContainsAny(OcelotFeature.IPC, OcelotFeature.All))
        {
            Logger.Info("Initializing IPC Manager...");
            ipc.Initialize();
        }


        modules?.PostInitialize();

        Svc.Framework.Update += Tick;
        Svc.Chat.ChatMessage += OnChatMessage;
        Svc.ClientState.TerritoryChanged += OnTerritoryChanged;
    }

    public virtual bool ShouldTick()
    {
        return true;
    }

    public virtual void Tick(IFramework framework)
    {
        if (!ShouldTick())
        {
            return;
        }

        modules?.Tick(framework);
    }

    public virtual void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        modules?.OnChatMessage(type, timestamp, sender, message, isHandled);
    }

    public virtual void OnTerritoryChanged(ushort id)
    {
        modules?.OnTerritoryChanged(id);
    }

    public virtual void Dispose()
    {
        if (modules != null)
        {
            Svc.PluginInterface.UiBuilder.Draw -= modules.Draw;
            modules.Dispose();
        }

        windows?.Dispose();
        commands?.Dispose();

        Svc.Framework.Update -= Tick;
        Svc.Chat.ChatMessage -= OnChatMessage;
        Svc.ClientState.TerritoryChanged -= OnTerritoryChanged;

        PictoService.Dispose();
        ECommonsMain.Dispose();
    }
}
