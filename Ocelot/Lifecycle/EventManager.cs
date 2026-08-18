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
            logger.Debug($"Starting host: {host.GetType().FullName}");
            host.Start();
            logger.Debug($"Finished starting host: {host.GetType().FullName}");
        }
    }

    public void Stop()
    {
        hosts.Reverse();
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
