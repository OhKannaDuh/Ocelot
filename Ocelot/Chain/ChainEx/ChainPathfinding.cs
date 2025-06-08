using System.Numerics;
using ECommons.Automation.NeoTaskManager;
using ECommons.Throttlers;
using Ocelot.IPC;

namespace Ocelot.Chain.ChainEx;

public static class ChainPathfinding
{
    private static TaskManagerTask WaitToStartPathfinding(VNavmesh vnav)
    {
        return new(() =>
        {
            if (EzThrottler.Throttle($"ChainPathfinding.WaitToStartPathfinding", 50))
            {
                return vnav.IsRunning();
            }

            return false;
        }, new() { TimeLimitMS = 2000 });
    }

    public static Chain WaitToStartPathfinding(this Chain chain, VNavmesh vnav)
    {
        return chain.Then(WaitToStartPathfinding(vnav));
    }

    private static TaskManagerTask WaitToStopPathfinding(VNavmesh vnav)
    {
        return new(() =>
        {
            if (EzThrottler.Throttle($"ChainPathfinding.WaitToStopPathfinding", 50))
            {
                return !vnav.IsRunning();
            }

            return false;
        }, new() { TimeLimitMS = 2000 });
    }

    public static Chain WaitToStopPathfinding(this Chain chain, VNavmesh vnav)
    {
        return chain.Then(WaitToStopPathfinding(vnav));
    }

    public static Chain WaitForPathfindingCycle(this Chain chain, VNavmesh vnav)
    {
        return chain.WaitToStartPathfinding(vnav).WaitToStopPathfinding(vnav);
    }

    public static Chain PathfindAndMoveTo(this Chain chain, VNavmesh vnav, Vector3 destination)
    {
        return chain.Then(_ => vnav.PathfindAndMoveTo(destination, false));
    }
}
