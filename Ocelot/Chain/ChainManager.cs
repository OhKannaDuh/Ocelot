using System.Collections.Generic;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;

namespace Ocelot.Chain;

public static class ChainManager
{
    private static readonly Dictionary<string, ChainQueue> queues = new();

    private static bool Initialized;

    public static IReadOnlyDictionary<string, ChainQueue> Queues {
        get => queues;
    }

    public static ChainQueue Get(string id)
    {
        lock (queues)
        {
            if (!queues.TryGetValue(id, out var q))
            {
                q = new ChainQueue();
                queues[id] = q;
            }

            return q;
        }
    }

    internal static void Initialize()
    {
        if (Initialized)
        {
            return;
        }

        Initialized = true;
        Svc.Framework.Update += Tick;
    }

    private static void Tick(IFramework framework)
    {
        List<string>? toRemove = null;

        lock (queues)
        {
            foreach (var (id, q) in queues)
            {
                q.Tick(framework);

                if (q is { IsRunning: false, QueueCount: 0, TimeAlive.TotalSeconds: >= 1 })
                {
                    q.Dispose();
                    (toRemove ??= new List<string>()).Add(id);
                }
            }

            if (toRemove is not null)
            {
                foreach (var id in toRemove)
                {
                    queues.Remove(id);
                }
            }
        }
    }

    public static void AbortAll()
    {
        lock (queues)
        {
            foreach (var q in queues.Values)
            {
                q.Abort();
            }
        }
    }

    public static void Close()
    {
        Svc.Framework.Update -= Tick;
        lock (queues)
        {
            foreach (var q in queues.Values)
            {
                q.Dispose();
            }

            queues.Clear();
            Initialized = false;
        }
    }
}
