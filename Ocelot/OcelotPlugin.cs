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

    public abstract IOcelotConfig OcelotConfig { get; }

    public readonly ModuleManager Modules = new();

    public readonly WindowManager Windows = new();

    public readonly CommandManager Commands = new();

    public readonly IPCManager IPC = new();

    private List<OcelotFeature> enabledFeatures = [];

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
            enabledFeatures.Add(OcelotFeature.All);
        }
        else
        {
            enabledFeatures = features.ToList();
        }

        if (enabledFeatures.ContainsAny(OcelotFeature.ModuleManager, OcelotFeature.All))
        {
            Logger.Info("Initializing Module Manager...");
            Modules.AutoRegister(this, OcelotConfig);
            Modules.PreInitialize();
            Modules.Initialize();
            Svc.PluginInterface.UiBuilder.Draw += Modules.Render;
        }

        if (enabledFeatures.ContainsAny(OcelotFeature.WindowManager, OcelotFeature.All))
        {
            Logger.Info("Initializing Window Manager...");
            Windows.Initialize(this, OcelotConfig);
        }

        if (enabledFeatures.ContainsAny(OcelotFeature.CommandManager, OcelotFeature.All))
        {
            Logger.Info("Initializing Command Manager...");
            Commands.Initialize(this);
        }

        if (enabledFeatures.ContainsAny(OcelotFeature.IPC, OcelotFeature.All))
        {
            Logger.Info("Initializing IPC Manager...");
            IPC.Initialize();
        }


        Modules.PostInitialize();

        Svc.Framework.Update += Update;
        Svc.Chat.ChatMessage += OnChatMessage;
        Svc.ClientState.TerritoryChanged += OnTerritoryChanged;
    }

    public virtual bool ShouldUpdate()
    {
        return true;
    }

    protected virtual void Update(IFramework framework)
    {
        if (!ShouldUpdate())
        {
            return;
        }

        Modules.PreUpdate(framework);
        Modules.Update(framework);
        Modules.PostUpdate(framework);
    }

    protected virtual void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        Modules.OnChatMessage(type, timestamp, sender, message, isHandled);
    }

    protected virtual void OnTerritoryChanged(ushort id)
    {
        Modules.OnTerritoryChanged(id);
    }

    public virtual void Dispose()
    {
        Svc.PluginInterface.UiBuilder.Draw -= Modules.Render;
        Modules.Dispose();

        Windows.Dispose();
        Commands.Dispose();

        Svc.Framework.Update -= Update;
        Svc.Chat.ChatMessage -= OnChatMessage;
        Svc.ClientState.TerritoryChanged -= OnTerritoryChanged;

        PictoService.Dispose();
        ECommonsMain.Dispose();
    }
}
