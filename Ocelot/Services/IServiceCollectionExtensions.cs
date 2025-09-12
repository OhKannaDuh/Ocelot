using System;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Lifecycle;
using Ocelot.Lifecycle.Hosts;
using Ocelot.Services.Data;
using Ocelot.Services.Data.Cache;
using Ocelot.Services.Logger;

namespace Ocelot.Services;

public static class IServiceCollectionExtensions
{
    internal static void LoadOcelotCore(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerService, NullLoggerService>();

        services.AddSingleton(typeof(ICache<,>), typeof(GenericCache<,>));
        services.AddSingleton(typeof(ICache<,>), typeof(ExcelCache<,>));
        services.AddSingleton(typeof(IDataRepository<>), typeof(ExcelDataRepository<>));

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
        services.AddSingleton<IDataManager>(c => c.GetRequiredService<DalamudServices>().Data);
    }
}
