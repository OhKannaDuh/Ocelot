using System;
using System.Linq;
using Dalamud.Game;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.DalamudServices;
using Ocelot.Chain;
using Ocelot.Commands;
using Ocelot.Intents;
using Ocelot.Modules;
using Ocelot.Services;
using Ocelot.Services.Ipc;
using Ocelot.Services.Pathfinding;
using Ocelot.Services.Translation;
using Ocelot.Services.Windows;
using Ocelot.Windows;
using Pictomancy;

namespace Ocelot;

public abstract class OcelotPlugin : IDalamudPlugin
{
    public abstract string Name { get; }

    internal static OcelotPlugin Plugin { get; private set; } = null!;

    public virtual string Version {
        get => Svc.PluginInterface.Manifest.AssemblyVersion.ToString();
    }

    public const string OcelotVersion = "1.2.0";

    public abstract OcelotConfig OcelotConfig { get; }

    public static IIpcManager IpcManager {
        get => OcelotServices.GetCached<IIpcManager>();
    }

    public static IWindowManager WindowManager {
        get => OcelotServices.GetCached<IWindowManager>();
    }

    public readonly CommandManager Commands = new();

    protected readonly PluginWatcher PluginWatcher;

    protected readonly EventManager EventManager = new();

    protected readonly RenderScheduler RenderScheduler = new();

    protected readonly UpdateScheduler UpdateScheduler = new();

    public RenderContext? RenderContext { get; private set; }

    protected OcelotPlugin(IDalamudPluginInterface plugin, params Module[] eModules)
    {
        Plugin = this;
        ECommonsMain.Init(plugin, this, eModules);
        PictoService.Initialize(plugin);

        Registry.RegisterAssemblies(typeof(OcelotPlugin).Assembly);
        Registry.RegisterAssemblies(GetType().Assembly);

        PluginWatcher = new PluginWatcher();
    }

    protected void OcelotInitialize()
    {
        if (OcelotConfig.OcelotCoreConfig.Language == "")
        {
            OcelotConfig.OcelotCoreConfig.Language = Svc.ClientState.ClientLanguage switch {
                ClientLanguage.French => "fr",
                ClientLanguage.German => "de",
                ClientLanguage.Japanese => "jp",
#if DALAMUD_CN
                ClientLanguage.ChineseSimplified  => "zh",
#endif
                _ => "en",
            };

            OcelotConfig.Save();
        }

        Svc.Framework.RunOnFrameworkThread(LoadCore);
    }

    protected abstract void RegisterPluginTypes();

    private void LoadCore()
    {
        Logger.Debug("Loading Core");

        OcelotServices.Container.AddSingleton(this);
        OcelotServices.Container.AddSingleton(OcelotConfig);
        RegisterPluginTypes();

        OcelotServices.Initialize(this, OcelotConfig);

        OcelotServices.GetCached<ITranslationService>().SetLanguage(OcelotConfig.OcelotCoreConfig.Language, "en");

        Svc.PluginInterface.UiBuilder.Draw += Render;
        Svc.Framework.Update += Update;
        PluginWatcher.OnPluginListChanged += OnPluginListChanged;

        IpcManager.Refresh();
        WindowManager.Initialize();

        Commands.Initialize(this);
        ChainManager.Initialize();

        var provider = OcelotServices.Container.Get<IPathfinderServiceProvider>();
        var service = OcelotServices.Container.Get<PathfinderService>();
        service.SetPathfinderService(provider.GetService());

        EventManager.Initialize();
        EventManager.Refresh();
        RenderScheduler.Refresh();
        UpdateScheduler.Refresh();


        var candidates = OcelotServices.Container.GetAll<IInitializable>().ToList();
        foreach (var candidate in candidates)
        {
            candidate.PreInitialize();
        }

        foreach (var candidate in candidates)
        {
            candidate.Initialize();
        }

        foreach (var candidate in candidates)
        {
            candidate.PostInitialize();
        }

        OcelotServices.Container.OnServiceChanged += OnServicesChanged;

        OnCoreLoaded();
    }

    protected virtual void OnCoreLoaded() { }

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
        UpdateScheduler.Update(context);
    }

    protected virtual void Render()
    {
        var shouldRenderThisFrame = ShouldRender();
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
        if (!shouldRenderThisFrame)
        {
            return;
        }

        RenderScheduler.Render(RenderContext);

        if (!shouldRenderThisFrame)
        {
            return;
        }

        try
        {
            PictoService.GetDrawList().Dispose();
        }
        catch (InvalidOperationException) { }
    }

    private void OnServicesChanged(object? sender, ServiceChangedEventContext context)
    {
        foreach (var candidate in OcelotServices.Container.GetAll<IServiceWatcher>())
        {
            candidate.OnServicesChanged(this, context);
        }

        // @todo think about adding these to di and adding the IServiceWatcher Intent
        EventManager.Refresh();
        RenderScheduler.Refresh();
        UpdateScheduler.Refresh();
    }

    private void OnPluginListChanged()
    {
        IpcManager.Refresh();

        var provider = OcelotServices.Container.Get<IPathfinderServiceProvider>();
        var service = OcelotServices.Container.Get<PathfinderService>();

        service.SetPathfinderService(provider.GetService());
    }

    public virtual void Dispose()
    {
        Svc.PluginInterface.UiBuilder.Draw -= Render;
        PluginWatcher.OnPluginListChanged -= OnPluginListChanged;
        Svc.Framework.Update -= Update;

        WindowManager.Dispose();
        Commands.Dispose();
        ChainManager.Close();

        EventManager.Dispose();

        PictoService.Dispose();
        ECommonsMain.Dispose();
        OcelotServices.Dispose();
    }
}
