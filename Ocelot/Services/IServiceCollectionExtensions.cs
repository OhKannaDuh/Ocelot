using System;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Ipc.VNavmesh;
using Ocelot.Lifecycle;
using Ocelot.Lifecycle.Hosts;
using Ocelot.Services.ClientState;
using Ocelot.Services.Data;
using Ocelot.Services.Data.Cache;
using Ocelot.Services.Gate;
using Ocelot.Services.Logger;
using Ocelot.Services.PlayerState;
using Ocelot.Services.PluginStatus;
using Ocelot.Services.WindowManager;
using Ocelot.Windows;

namespace Ocelot.Services;

public static class IServiceCollectionExtensions
{
    internal static void LoadOcelotCore(this IServiceCollection services)
    {
        services.AddSingleton<ILogger, NullLogger>();
        services.AddSingleton<IGateService, GateService>();
        
        services.AddSingleton<IWindowManager, WindowManager.WindowManager>();
        services.AddSingleton<IMainWindow, MainWindow>();
        services.AddSingleton<IMainRenderer, NullMainRenderer>();
        services.AddSingleton<IConfigWindow, ConfigWindow>();
        services.AddSingleton<IConfigRenderer, NullConfigRenderer>();

        services.AddSingleton(typeof(ICache<,>), typeof(GenericCache<,>));
        services.AddSingleton(typeof(ICache<,>), typeof(ExcelCache<,>));
        services.AddSingleton(typeof(IDataRepository<,>), typeof(DataRepository<,>));
        services.AddSingleton(typeof(IDataRepository<>), typeof(ExcelDataRepository<>));
        
        services.AddSingleton<IClient, ClientState.Client>();
        services.AddSingleton<IPlayer, PlayerState.Player>();
        services.AddSingleton<IPluginStatus, PluginStatus.PluginStatus>();

        services.AddSingleton<IVNavmeshIpc, VNavmeshIpc>();

        services.AddSingleton<IEventHost, StartHost>();
        services.AddSingleton<IEventHost, UpdateHost>();
        services.AddSingleton<IEventHost, OverlayRenderHost>();
        services.AddSingleton<IEventHost, StopHost>();

        services.AddSingleton<EventManager>();
    }

    internal static void LoadDalamudServices(this IServiceCollection services, IDalamudPluginInterface plugin)
    {
        var dalamudServices = plugin.Create<DalamudServices>();
        if (dalamudServices == null)
        {
            throw new Exception("Unable to create Dalamud Services");
        }

        services.AddSingleton(dalamudServices);

        services.AddSingleton<IFramework>(c => c.GetRequiredService<DalamudServices>().Framework);
        services.AddSingleton<IDataManager>(c => c.GetRequiredService<DalamudServices>().DataManager);
        services.AddSingleton<Dalamud.Plugin.Services.IClientState>(c => c.GetRequiredService<DalamudServices>().ClientState);
        services.AddSingleton<ITargetManager>(c => c.GetRequiredService<DalamudServices>().TargetManager);
    }
}
