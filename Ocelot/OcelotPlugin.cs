using Dalamud.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Lifecycle;
using Ocelot.Services;

namespace Ocelot;

public abstract class OcelotPlugin : IDalamudPlugin
{
    private readonly ServiceProvider services;

    private readonly EventManager host;

    public abstract string Name { get; }

    protected OcelotPlugin(IDalamudPluginInterface plugin)
    {
        Registry.RegisterAssemblies(typeof(OcelotPlugin).Assembly, GetType().Assembly);

        var collection = new OcelotServiceCollection();

        collection.AddSingleton(plugin);
        collection.AddSingleton<IDalamudPlugin>(this);
        collection.AddSingleton(this);

        collection.LoadDalamudServices(plugin);
        collection.LoadOcelotCore();
        collection.AutoDiscover();

        Boostrap(collection);

        services = collection.Build();

        host = services.GetRequiredService<EventManager>();
        host.Start();
    }

    protected abstract void Boostrap(IServiceCollection collection);

    public void Dispose()
    {
        host.Stop();
        services.Dispose();
    }
}
