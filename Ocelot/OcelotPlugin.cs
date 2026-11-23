using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Lifecycle;
using Ocelot.Services;
using Ocelot.Services.Translation;
using System.Threading;
using System.Threading.Tasks;

namespace Ocelot;

public abstract class OcelotPlugin : IAsyncDalamudPlugin
{
    private readonly IDalamudPluginInterface plugin;
    private ServiceProvider? services;
    private EventManager? host;

    public abstract string Name { get; }

    protected OcelotPlugin(IDalamudPluginInterface plugin, IPluginLog logger)
    {
        this.plugin = plugin;
    }

    protected abstract void Bootstrap(IServiceCollection collection);

    public Task LoadAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Registry.RegisterAssemblies(typeof(OcelotPlugin).Assembly, GetType().Assembly);

        var collection = new OcelotServiceCollection();

        collection.AddSingleton(plugin);
        collection.AddSingleton(this);

        collection.AddSingleton(new TranslatorContextResolverOptions(GetType()));

        collection.LoadDalamudServices(plugin);
        collection.LoadOcelotCore();

        Bootstrap(collection);

        try
        {
            services = collection.Build();
            host = services.GetRequiredService<EventManager>();
            host.Start();
        }
        catch
        {
            ShutDown();
            throw;
        }

        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        ShutDown();
        return ValueTask.CompletedTask;
    }

    // Without the finally a throwing Stop left hooks, windows and IPC alive after unload.
    private void ShutDown()
    {
        try
        {
            host?.Stop();
        }
        finally
        {
            host = null;
            services?.Dispose();
            services = null;
        }
    }
}
