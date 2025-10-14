using Ocelot.Lifecycle.Hosts;

namespace Ocelot.Lifecycle;

public class EventManager(IEnumerable<IEventHost> hosts) : IDisposable
{
    private readonly IEventHost[] hosts = hosts.Where(h => h.Count > 0).OrderByDescending(h => h.Order).ToArray();

    private bool disposed = false;

    public void Start()
    {
        foreach (var host in hosts)
        {
            host.Start();
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
