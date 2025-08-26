using System;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.DalamudServices;
using Ocelot.Chain;
using Ocelot.Commands;
using Ocelot.Data;
using Ocelot.Debug;
using Ocelot.Gameplay.Mechanic;
using Ocelot.Gameplay.Rotation;
using Ocelot.Gameplay.Targeting;
using Ocelot.IPC;
using Ocelot.Modules;
using Ocelot.Windows;
using Pictomancy;

namespace Ocelot;

public abstract class OcelotPlugin : IDalamudPlugin
{
    public abstract string Name { get; }

    internal static OcelotPlugin Plugin { get; private set; } = null!;

    public virtual string Version
    {
        get => Svc.PluginInterface.Manifest.AssemblyVersion.ToString();
    }

    public const string OcelotVersion = "1.1.0";

    public abstract OcelotConfig OcelotConfig { get; }

    public readonly ModuleManager Modules = new();

    public readonly WindowManager Windows = new();

    public readonly CommandManager Commands = new();

    public readonly IPCManager IPC = new();

    protected readonly PluginWatcher PluginWatcher;

    public RenderContext? RenderContext { get; private set; } = null;

    private bool shouldRenderThisFrame = false;

    protected OcelotPlugin(IDalamudPluginInterface plugin, params Module[] eModules)
    {
        Plugin = this;
        ECommonsMain.Init(plugin, this, eModules);
        PictoService.Initialize(plugin);

        Registry.RegisterAssemblies(typeof(OcelotPlugin).Assembly);
        Registry.RegisterAssemblies(GetType().Assembly);

        PluginWatcher = new PluginWatcher();
    }

    protected void OcelotInitialize(params OcelotFeature[] features)
    {
        var lang = OcelotConfig.OcelotCoreConfig.Language;
        if (lang == "")
        {
            lang = Svc.ClientState.ClientLanguage switch
            {
                ClientLanguage.French => "fr",
                ClientLanguage.German => "de",
                ClientLanguage.Japanese => "jp",
#if DALAMUD_CN
                ClientLanguage.ChineseSimplified  => "zh",
#endif
                _ => "en",
            };

            OcelotConfig.OcelotCoreConfig.Language = lang;
            OcelotConfig.Save();
        }

        if (!I18N.HasLanguage(lang))
        {
            lang = "en";
        }

        I18N.SetLanguage(lang);

        Svc.Framework.RunOnFrameworkThread(() => LoadCore(features));
    }

    private void LoadCore(params OcelotFeature[] features)
    {
        Logger.Debug("Loading Core");

        Svc.PluginInterface.UiBuilder.Draw += PreRender;

        OcelotFeatureEx.SetFeatures(features.Length <= 0 ? [OcelotFeature.All] : features);

        Svc.Framework.Update += Update;
        Svc.Chat.ChatMessage += OnChatMessage;
        Svc.ClientState.TerritoryChanged += OnTerritoryChanged;

        Svc.PluginInterface.UiBuilder.Draw += PostRender;

        PluginWatcher.OnPluginListChanged += OnPluginListChanged;

        if (PluginWatcher.IsOcelotPluginEnabled(OcelotPlugins.OcelotMonitor))
        {
            IPC.AddProvider(new DebugIPCProvider(this));
        }

        if (OcelotFeature.IPC.IsEnabled())
        {
            Logger.Info("Initializing IPC Manager...");
            IPC.Initialize();
        }

        if (OcelotFeature.ModuleManager.IsEnabled())
        {
            Logger.Info("Initializing Module Manager...");
            Modules.AutoRegister(this, OcelotConfig);
            Modules.PreInitialize();
            Modules.Initialize();
            Svc.PluginInterface.UiBuilder.Draw += Render;
        }

        if (OcelotFeature.WindowManager.IsEnabled())
        {
            Logger.Info("Initializing Window Manager...");
            Windows.Initialize(this, OcelotConfig);
        }

        if (OcelotFeature.CommandManager.IsEnabled())
        {
            Logger.Info("Initializing Command Manager...");
            Commands.Initialize(this);
        }

        if (OcelotFeature.Prowler.IsEnabled() && OcelotFeature.IPC.IsEnabled())
        {
            Logger.Info("Initializing Prowler...");
            Prowler.Prowler.Initialize(this);
        }

        if (OcelotFeature.ChainManager.IsEnabled())
        {
            ChainManager.Initialize();
        }

        if (OcelotFeature.RotationHelper.IsEnabled())
        {
            RotationHelper.Initialize(this);
        }

        if (OcelotFeature.MechanicHelper.IsEnabled())
        {
            MechanicHelper.Initialize(this);
        }

        if (OcelotFeature.TargetingHelper.IsEnabled())
        {
            TargetingHelper.Initialize(this);
        }

        Modules.InjectModules();
        Modules.InjectIPCs();
        Modules.PostInitialize();

        OnCoreLoaded();
    }

    protected virtual void OnCoreLoaded()
    {
    }

    protected virtual bool ShouldUpdate()
    {
        return true;
    }

    protected virtual bool ShouldRender()
    {
        return ShouldUpdate();
    }

    protected virtual void Update(IFramework framework)
    {
        if (!ShouldUpdate())
        {
            return;
        }

        var context = new UpdateContext(framework, this);

        PluginWatcher.Update(context);

        Modules.PreUpdate(context);
        Modules.Update(context);
        Modules.PostUpdate(context);
    }

    private void PreRender()
    {
        shouldRenderThisFrame = ShouldRender();
        if (!shouldRenderThisFrame)
        {
            return;
        }

        var draw = PictoService.Draw();
        if (draw == null)
        {
            RenderContext = null;
            return;
        }

        RenderContext = new RenderContext(this);
    }

    protected virtual void Render()
    {
        if (!shouldRenderThisFrame || RenderContext == null)
        {
            return;
        }

        Modules.Render(RenderContext);
    }

    private void PostRender()
    {
        if (!shouldRenderThisFrame || RenderContext == null)
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

    protected virtual void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        Modules.OnChatMessage(type, timestamp, sender, message, isHandled);
    }

    protected virtual void OnTerritoryChanged(ushort id)
    {
        Modules.OnTerritoryChanged(id);
    }

    private void OnPluginListChanged()
    {
        if (OcelotFeature.IPC.IsEnabled())
        {
            IPC.Initialize();
        }

        Modules.InjectIPCs();
    }

    public virtual void Dispose()
    {
        Svc.PluginInterface.UiBuilder.Draw -= PostRender;
        Svc.PluginInterface.UiBuilder.Draw -= Render;
        Svc.PluginInterface.UiBuilder.Draw -= PreRender;

        PluginWatcher.OnPluginListChanged -= OnPluginListChanged;

        Modules.PreDispose();
        Modules.Dispose();
        Modules.PostDispose();

        Windows.Dispose();
        IPC.Dispose();
        Commands.Dispose();
        ChainManager.Close();

        Svc.Framework.Update -= Update;
        Svc.Chat.ChatMessage -= OnChatMessage;
        Svc.ClientState.TerritoryChanged -= OnTerritoryChanged;

        RotationHelper.TearDown();
        MechanicHelper.TearDown();
        TargetingHelper.TearDown();

        PictoService.Dispose();
        ECommonsMain.Dispose();
    }
}
