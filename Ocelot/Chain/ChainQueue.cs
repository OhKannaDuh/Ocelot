using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;

namespace Ocelot.Chain;

public class ChainQueue : IDisposable
{
    private readonly Queue<Func<Chain>> chains = [];
    private Chain? chain = null;

    public void Submit(Func<Chain> factory)
    {
        lock (chains)
        {
            chains.Enqueue(factory);
        }
    }

    public void Submit(ChainFactory factory) => Submit(factory.Factory());

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

    public Chain? CurrentChain => chain;

    public void Tick(IFramework _)
    {
        if (chain != null && !chain.IsComplete())
            return;

        lock (chains)
        {
            if (chains.Count == 0)
            {
                chain = null;
                return;
            }

            Logger.Debug("Starting next chain...");
            var factory = chains.Dequeue();
            chain = factory();
        }
    }

    public bool IsRunning => chain != null && !chain.IsComplete();

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
