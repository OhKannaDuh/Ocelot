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


    public RenderContext? RenderContext { get; private set; } = null;

    public OcelotPlugin(IDalamudPluginInterface plugin, params Module[] eModules)
    {
        ECommonsMain.Init(plugin, this, eModules);
        PictoService.Initialize(plugin);

        Registry.RegisterAssemblies(typeof(OcelotPlugin).Assembly);
        Registry.RegisterAssemblies(GetType().Assembly);
    }

    protected void OcelotInitialize(params OcelotFeature[] features)
    {
        Svc.PluginInterface.UiBuilder.Draw += PreRender;

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
            Svc.PluginInterface.UiBuilder.Draw += Render;
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

        Svc.PluginInterface.UiBuilder.Draw += PostRender;
    }

    protected virtual bool ShouldUpdate()
    {
        return true;
    }

    protected virtual void Update(IFramework framework)
    {
        if (!ShouldUpdate())
        {
            return;
        }

        var context = new UpdateContext(framework, this);

        Modules.PreUpdate(context);
        Modules.Update(context);
        Modules.PostUpdate(context);
    }

    protected virtual void Render()
    {
        if (RenderContext == null)
        {
            return;
        }

        Modules.Render(RenderContext);
    }

    protected virtual void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        Modules.OnChatMessage(type, timestamp, sender, message, isHandled);
    }

    protected virtual void OnTerritoryChanged(ushort id)
    {
        Modules.OnTerritoryChanged(id);
    }

    private void PreRender()
    {
        var draw = PictoService.Draw(); // Start draw
        if (draw == null)
        {
            RenderContext = null;
            return;
        }

        RenderContext = new RenderContext(this);
    }

    private void PostRender()
    {
        if (RenderContext == null)
        {
            return;
        }

        try
        {
            PictoService.GetDrawList().Dispose();
        }
        catch (InvalidOperationException)
        {
        }
    }

    public virtual void Dispose()
    {
        Svc.PluginInterface.UiBuilder.Draw += PostRender;
        Svc.PluginInterface.UiBuilder.Draw -= Render;
        Modules.Dispose();

        Windows.Dispose();
        Commands.Dispose();

        Svc.Framework.Update -= Update;
        Svc.Chat.ChatMessage -= OnChatMessage;
        Svc.ClientState.TerritoryChanged -= OnTerritoryChanged;

        Svc.PluginInterface.UiBuilder.Draw += PreRender;

        PictoService.Dispose();
        ECommonsMain.Dispose();
    }
}
