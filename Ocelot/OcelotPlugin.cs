using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Lifecycle;
using Ocelot.Services;
using Ocelot.Services.Translation;

namespace Ocelot;

public abstract class OcelotPlugin : IDalamudPlugin
{
    private readonly ServiceProvider services;

    private readonly EventManager host;

    public abstract string Name { get; }

    protected OcelotPlugin(IDalamudPluginInterface plugin, IPluginLog logger)
    {
        Registry.RegisterAssemblies(typeof(OcelotPlugin).Assembly, GetType().Assembly);

        var collection = new OcelotServiceCollection();

        collection.AddSingleton(plugin);
        collection.AddSingleton<IDalamudPlugin>(this);
        collection.AddSingleton(this);

        collection.AddSingleton(new TranslatorContextResolverOptions(GetType()));

        collection.LoadDalamudServices(plugin);
        collection.LoadOcelotCore();
        collection.AutoDiscover();

        Boostrap(collection);

        logger.Debug("KA");
        services = collection.Build();
        logger.Debug("KB");

        logger.Debug("LA");
        host = services.GetRequiredService<EventManager>();
        logger.Debug("LB");

        logger.Debug("MA");
        host.Start();
        logger.Debug("MB");
    }

    protected abstract void Boostrap(IServiceCollection collection);

    public void Dispose()
    {
        host.Stop();
        services.Dispose();
    }
}
