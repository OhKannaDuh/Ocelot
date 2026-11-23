using Ocelot.Lifecycle.Hosts;
using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle;

public class EventManager(IEnumerable<IEventHost> hosts, ILogger<EventManager> logger) : IDisposable
{
    private readonly IEventHost[] hosts = hosts.OrderByDescending(h => h.Order).ToArray();

    private bool disposed = false;

    public void Start()
    {
        foreach (var host in hosts)
        {
            logger.Verbose($"Starting host: {host.GetType().FullName}");
            host.Start();
            logger.Verbose($"Finished starting host: {host.GetType().FullName}");
        }
    }

    public void Stop()
    {
        // Start order on purpose: StopHost (Order MinValue) must run last, after the overlay host unhooks Draw;
        // stopping it first disposed Pictomancy while a frame could still draw, which crashed the game on unload.
        foreach (var host in hosts)
        {
            host.Stop();
        }

        disposed = true;
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        Stop();
    }
}
