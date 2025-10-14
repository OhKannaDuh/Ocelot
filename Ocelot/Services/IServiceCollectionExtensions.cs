using Dalamud.Game.ClientState.Objects;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Config;
using Ocelot.Config.Fields;
using Ocelot.Config.Renderers;
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
using Ocelot.Services.Translation;
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

        services.AddSingleton<IConfigSaver, ConfigSaver>();
        services.AddSingleton<IFieldRenderer<CheckboxAttribute>, CheckboxRenderer>();
        services.AddSingleton<IFieldRenderer<FloatRangeAttribute>, FloatRangeRenderer>();
        services.AddSingleton<IFieldRenderer<IntRangeAttribute>, IntRangeRenderer>();

        services.AddSingleton<ITranslationRepository, TranslationRepository>();
        services.AddSingleton<ITranslator, Translator>();

        services.AddSingleton(typeof(ICache<,>), typeof(GenericCache<,>));
        services.AddSingleton(typeof(ICache<,>), typeof(ExcelCache<,>));
        services.AddSingleton(typeof(IDataRepository<,>), typeof(DataRepository<,>));
        services.AddSingleton(typeof(IDataRepository<>), typeof(ExcelDataRepository<>));

        services.AddSingleton<IClient, Client>();
        services.AddSingleton<IPlayer, Player>();
        services.AddSingleton<IPluginStatus, PluginStatus.PluginStatus>();

        services.AddSingleton<IVNavmeshIpc, VNavmeshIpc>();

        services.AddSingleton<IEventHost, LoadHost>();
        services.AddSingleton<IEventHost, StartHost>();
        services.AddSingleton<IEventHost, UpdateHost>();
        services.AddSingleton<IEventHost, OverlayRenderHost>();
        services.AddSingleton<IEventHost, TerritoryHost>();
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
        services.AddSingleton<IClientState>(c => c.GetRequiredService<DalamudServices>().ClientState);
        services.AddSingleton<ITargetManager>(c => c.GetRequiredService<DalamudServices>().TargetManager);
        services.AddSingleton<ITextureProvider>(c => c.GetRequiredService<DalamudServices>().TextureProvider);
    }

    public static void AddConfig<T, P>(this IServiceCollection services, Func<P, T> selector)
        where T : class, IAutoConfig where P : notnull
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(selector);

        services.AddSingleton<T>(sp => selector(sp.GetRequiredService<P>()));
        services.AddSingleton<IAutoConfig>(sp => sp.GetRequiredService<T>());
    }
}
