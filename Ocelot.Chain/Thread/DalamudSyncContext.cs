using System.Collections.Concurrent;

namespace Ocelot.Chain.Thread;

internal sealed class DalamudSyncContext(ConcurrentQueue<Action> queue) : SynchronizationContext
{
    public override void Post(SendOrPostCallback d, object? state)
    {
        queue.Enqueue(() => d(state));
    }
}
