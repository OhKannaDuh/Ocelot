using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;

namespace Ocelot.Chain;

public class ChainQueue : IDisposable
{
    private readonly LinkedList<Func<Chain>> chains = new();

    private Chain? chain = null;

    public bool hasRun { get; private set; } = false;

    public int aliveTime { get; private set; } = 0;

    public void Submit(Func<Chain> factory)
    {
        hasRun = true;
        lock (chains)
        {
            chains.AddLast(factory);
        }
    }

    public void SubmitFront(Func<Chain> factory)
    {
        hasRun = true;
        lock (chains)
        {
            chains.AddFirst(factory);
        }
    }

    public void Submit(ChainFactory factory)
    {
        Submit(factory.Factory());
    }

    public void SubmitFront(ChainFactory factory)
    {
        SubmitFront(factory.Factory());
    }

    public void Abort()
    {
        lock (chains)
        {
            if (chain != null)
            {
                chain.Abort();
                chain = null;
            }

            chains.Clear();
        }

        Logger.Debug("Aborted current chain and cleared the queue.");
    }

    public void Clear()
    {
        lock (chains)
        {
            chains.Clear();
        }
    }

    public Chain? CurrentChain
    {
        get => chain;
    }

    public void Tick(IFramework framework)
    {
        aliveTime += framework.UpdateDelta.Milliseconds;

        if (chain != null && !chain.IsComplete())
        {
            return;
        }

        lock (chains)
        {
            if (chains.Count == 0)
            {
                chain = null;
                return;
            }

            var factory = chains.First!.Value;
            chains.RemoveFirst();
            chain = factory();
        }
    }

    public bool IsRunning
    {
        get => chain != null && !chain.IsComplete();
    }

    public int QueueCount
    {
        get
        {
            lock (chains)
            {
                return chains.Count;
            }
        }
    }

    public void Dispose()
    {
        if (chain != null)
        {
            chain.Abort();
        }

        Clear();
    }
}
