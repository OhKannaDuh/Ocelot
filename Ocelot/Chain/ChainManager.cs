using System.Collections.Generic;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;

namespace Ocelot.Chain;

public static class ChainManager
{
    private static readonly Dictionary<string, ChainQueue> Queues = [];

    private static bool Initialized;

    public static ChainQueue Get(string id)
    {
        lock (Queues)
        {
            if (!Queues.TryGetValue(id, out var queue))
            {
                queue = new ChainQueue();
                Queues[id] = queue;
            }

            return queue;
        }
    }

    public static Dictionary<string, ChainQueue> Active()
    {
        return Queues;
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
        lock (Queues)
        {
            var toRemove = new List<string>();

            foreach (var pair in Queues)
            {
                var id = pair.Key;
                var queue = pair.Value;

                queue.Tick(framework);

                if (queue is { IsRunning: false, QueueCount: 0, aliveTime: >= 1000 })
                {
                    if (queue.hasRun)
                    {
                        Logger.Debug($"Disposing ChainQueue '{id}' (inactive and empty)");
                    }

                    queue.Dispose();
                    toRemove.Add(id);
                }
            }

            foreach (var id in toRemove)
            {
                Queues.Remove(id);
            }
        }
    }

    public static void AbortAll()
    {
        lock (Queues)
        {
            foreach (var queue in Queues.Values)
            {
                queue.Abort();
            }
        }

        Logger.Debug("Aborted all active ChainQueues.");
    }

    public static void Close()
    {
        Svc.Framework.Update -= Tick;

        lock (Queues)
        {
            foreach (var queue in Queues.Values)
            {
                queue.Dispose();
            }

            Queues.Clear();
            Initialized = false;
        }
    }
}
