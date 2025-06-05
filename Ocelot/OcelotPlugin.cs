using System;
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


namespace Ocelot;

public abstract class OcelotPlugin : IDalamudPlugin
{
    public abstract string Name { get; }

    public abstract IOcelotConfig _config { get; }

    public ModuleManager? modules = null;

    public WindowManager? windows = null;

    public CommandManager? commands = null;

    public IPCManager? ipc = null;

    private List<OcelotFeature> features = [];

    public OcelotPlugin(IDalamudPluginInterface plugin, params Module[] eModules)
    {
        ECommonsMain.Init(plugin, this, eModules);

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
            modules = new();
            modules.AutoRegister(this, _config);
            modules.Initialize(this, _config);
            Svc.PluginInterface.UiBuilder.Draw += modules.Draw;
        }

        if (this.features.ContainsAny(OcelotFeature.WindowManager, OcelotFeature.All))
        {
            Logger.Info("Initializing Window Manager...");
            windows = new();
            windows.Initialize(this, _config);
        }

        if (this.features.ContainsAny(OcelotFeature.CommandManager, OcelotFeature.All))
        {
            Logger.Info("Initializing Command Manager...");
            commands = new();
            commands.Initialize(this);
        }

        if (this.features.ContainsAny(OcelotFeature.IPC, OcelotFeature.All))
        {
            Logger.Info("Initializing IPC Manager...");
            ipc = new();
            ipc.Initialze();
        }

        Svc.Framework.Update += Tick;
        Svc.Chat.ChatMessage += OnChatMessage;
        Svc.ClientState.TerritoryChanged += OnTerritoryChanged;
    }

    public virtual bool ShouldTick() => true;

    public void Tick(IFramework framework)
    {
        if (!ShouldTick())
        {
            return;
        }

        modules?.Tick(framework);
    }

    public virtual void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled) => modules?.OnChatMessage(type, timestamp, sender, message, isHandled);

    public virtual void OnTerritoryChanged(ushort id) => modules?.OnTerritoryChanged(id);

    public void Dispose()
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

        ECommonsMain.Dispose();
    }
}
