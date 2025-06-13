using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;

namespace Ocelot.Chain;

public static class ChainManager
{
    private static readonly Dictionary<string, ChainQueue> queues = new();

    private static bool initialized = false;

    public static ChainQueue Get(string id)
    {
        lock (queues)
        {
            if (!queues.TryGetValue(id, out var queue))
            {
                queue = new ChainQueue();
                queues[id] = queue;
            }
            return queue;
        }
    }

    public static Dictionary<string, ChainQueue> Active() => queues;

    public static void Initialize()
    {
        if (initialized) return;
        initialized = true;

        Svc.Framework.Update += Tick;
    }

    private static void Tick(IFramework framework)
    {
        lock (queues)
        {
            var toRemove = new List<string>();

            foreach (var pair in queues)
            {
                var id = pair.Key;
                var queue = pair.Value;

                queue.Tick(framework);

                if (!queue.IsRunning && queue.QueueCount == 0 && queue.aliveTime >= 1000)
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
                queues.Remove(id);
            }
        }
    }

    public static void Close()
    {
        Svc.Framework.Update -= Tick;

        lock (queues)
        {
            foreach (var queue in queues.Values)
            {
                queue.Dispose();
            }
            queues.Clear();
            initialized = false;
        }
    }
}
