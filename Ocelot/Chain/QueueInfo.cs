using System;

namespace Ocelot.Chain;

public class QueueInfo
{
    public TimeSpan TimeAlive { get; init; }

    public int ChainsCompleted { get; init; }

    public int Pending { get; init; }

    public bool IsRunning { get; init; }

    public ChainInfo? CurrentChain { get; init; }

    public static QueueInfo FromQueue(ChainQueue queue)
    {
        return new QueueInfo {
            TimeAlive = queue.TimeAlive,
            ChainsCompleted = queue.ChainsCompleted,
            Pending = queue.QueueCount,
            IsRunning = queue.IsRunning,
            CurrentChain = queue.Current is { } r ? ChainInfo.FromRunner(r) : null,
        };
    }
}
