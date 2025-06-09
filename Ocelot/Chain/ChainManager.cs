using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;

namespace Ocelot.Chain;

public class ChainManager : IDisposable
{
    private static readonly Lazy<ChainManager> _instance = new(() => new ChainManager());

    private static ChainManager Instance => _instance.Value;

    private readonly Queue<Func<Chain>> chains = [];

    private Chain? chain = null;

    public static void Initialize()
    {
        Svc.Framework.Update += Tick;
    }

    public static void Submit(Func<Chain> factory)
    {
        lock (Instance.chains)
        {
            Instance.chains.Enqueue(factory);
        }
    }

    public static void Clear()
    {
        lock (Instance.chains)
        {
            Instance.chains.Clear();
        }
    }

    public static void Tick(IFramework framework)
    {
        if (Instance.chain != null && !Instance.chain.IsComplete())
        {
            return;
        }

        lock (Instance.chains)
        {
            if (Instance.chains.Count == 0)
            {
                Instance.chain = null;
                return;
            }

            var factory = Instance.chains.Dequeue();
            Instance.chain = factory();
        }
    }

    public static bool IsRunning()
    {
        return Instance.chain != null && !Instance.chain.IsComplete();
    }

    public static int GetChainQueueCount()
    {
        lock (Instance.chains)
        {
            return Instance.chains.Count;
        }
    }

    public static void Close()
    {
        Instance.Dispose();
    }

    public void Dispose()
    {
        Svc.Framework.Update -= Tick;
        if (chain != null)
        {
            chain.Abort();
        }

        Clear();
    }
}
