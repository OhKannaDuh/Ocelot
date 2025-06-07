using Ocelot.IPC;
using System.Numerics;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderPathfinding
{
    public static ChainBuilder WaitToStartPathfinding(this ChainBuilder builder, VNavmesh vnav)
        => builder
            .Debug($"Waiting to start vnavmesh")
            .Then(vnav.WaitToStart);

    public static ChainBuilder WaitToStopPathfinding(this ChainBuilder builder, VNavmesh vnav)
        => builder
            .Debug($"Waiting to stop vnavmesh")
            .Then(vnav.WaitToStart);

    public static ChainBuilder PathfindAndMoveTo(this ChainBuilder builder, VNavmesh vnav, Vector3 destination)
        => builder
            .Debug($"Moving to {destination}")
            .Then(() => vnav.PathfindAndMoveTo!(destination, false));

    public static ChainBuilder WaitForPathfindingCycle(this ChainBuilder builder, VNavmesh vnav)
        => builder
            .WaitToStartPathfinding(vnav)
            .WaitToStopPathfinding(vnav);
}
