using System.Reflection;
using Dalamud.Configuration;
using Dalamud.IoC;
using Dalamud.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocelot.Config;
using Ocelot.Config.Fields;
using Ocelot.Config.Renderers;
using Ocelot.Ipc.BossMod;
using Ocelot.Ipc.VNavmesh;
using Ocelot.Ipc.WrathCombo;
using Ocelot.Lifecycle;
using Ocelot.Lifecycle.Hosts;
using Ocelot.Services.ClientState;
using Ocelot.Services.Commands;
using Ocelot.Services.Commands.MainCommandDelegates;
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
        services.AddSingleton<ILogger, PluginLogger>();
        services.AddSingleton(typeof(ILogger<>), typeof(ContextualLogger<>));
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
        services.AddSingleton(typeof(ITranslator<>), typeof(ContextualTranslator<>));
        services.AddSingleton<ITranslatorContextResolver, TranslatorContextResolver>();

        services.AddSingleton(typeof(ICache<,>), typeof(GenericCache<,>));
        services.AddSingleton(typeof(ICache<,>), typeof(ExcelCache<,>));
        services.AddSingleton(typeof(IDataRepository<,>), typeof(DataRepository<,>));
        services.AddSingleton(typeof(IDataRepository<>), typeof(ExcelDataRepository<>));

        services.AddSingleton<IClient, Client>();
        services.AddSingleton<IPlayer, Player>();
        services.AddSingleton<IPluginStatus, PluginStatus.PluginStatus>();

        services.AddSingleton<IVNavmeshIpc, VNavmeshIpc>();
        services.AddSingleton<IBossModIpc, BossModIpc>();
        services.AddSingleton<IWrathComboIpc, WrathComboIpc>();

        services.AddSingleton<IEventHost, LoadHost>();
        services.AddSingleton<IEventHost, StartHost>();
        services.AddSingleton<IEventHost, UpdateHost>();
        services.AddSingleton<IEventHost, OverlayRenderHost>();
        services.AddSingleton<IEventHost, TerritoryHost>();
        services.AddSingleton<IEventHost, StopHost>();

        services.AddSingleton<EventManager>();

        services.AddSingleton<CommandManager>();

        // These commands don't get registered by ocelot as an IOcelotCommand, so CommandManager doesn't auto register them
        // But they this allows them to be DId into a main command delegate
        services.AddSingleton<ReloadTranslationsCommand>();

        services.AddSingleton<IConfigCommand, ConfigCommand>();
        services.AddSingleton<IOcelotCommand>(container => container.GetRequiredService<IConfigCommand>());

        services.AddSingleton<IMainCommand, MainCommand>();
        services.AddSingleton<IOcelotCommand>(container => container.GetRequiredService<IMainCommand>());
        services.AddSingleton<IMainCommandDelegate, ConfigDelegate>();
        services.AddSingleton<IMainCommandDelegate, ReloadTranslationsDelegate>();
    }

    internal static void LoadDalamudServices(this IServiceCollection services, IDalamudPluginInterface plugin)
    {
        var bag = plugin.Create<DalamudServices>() ?? throw new InvalidOperationException("Unable to create Dalamud Services");

        services.TryAddSingleton(bag);

        var properties = typeof(DalamudServices)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.GetCustomAttribute<PluginServiceAttribute>() != null);

        foreach (var prop in properties)
        {
            var serviceType = prop.PropertyType;

            services.TryAddSingleton(serviceType, sp =>
            {
                var svcBag = sp.GetRequiredService<DalamudServices>();
                var value = prop.GetValue(svcBag);
                if (value is null)
                {
                    throw new InvalidOperationException($"Dalamud service '{serviceType.Name}' is null on {nameof(DalamudServices)}.");
                }

                return value;
            });
        }
    }

    public static void AddConfig<TConcrete, TInterface>(this IServiceCollection services, IDalamudPluginInterface plugin)
        where TConcrete : class, TInterface, new()
        where TInterface : class, IPluginConfiguration
    {
        services.AddSingleton(plugin.GetPluginConfig() as TConcrete ?? new TConcrete());
        services.AddSingleton<TInterface>(s => s.GetRequiredService<TConcrete>());
        services.AddSingleton<IPluginConfiguration>(s => s.GetRequiredService<TConcrete>());

        var properties = typeof(TInterface).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach (var property in properties)
        {
            var prop = property;
            var propType = prop.PropertyType;

            services.AddSingleton(propType, sp =>
            {
                var cfg = sp.GetRequiredService<TInterface>();
                return prop.GetValue(cfg)!;
            });

            if (typeof(IAutoConfig).IsAssignableFrom(propType))
            {
                services.AddSingleton(typeof(IAutoConfig), sp =>
                {
                    var cfg = sp.GetRequiredService<TInterface>();
                    return prop.GetValue(cfg)!;
                });
            }
        }
    }
}
