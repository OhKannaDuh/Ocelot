using Ocelot.IPC;
using System.Numerics;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuildPathfinding
{
    public static ChainBuilder WaitToStartPathfinding(this ChainBuilder builder, VNavmesh vnav)
    {
        return builder
            .Debug($"Waiting to start vnavmesh")
            .Then(vnav.WaitToStart);
    }
    public static ChainBuilder WaitToStopPathfinding(this ChainBuilder builder, VNavmesh vnav)
    {
        return builder
            .Debug($"Waiting to start vnavmesh")
            .Then(vnav.WaitToStart);
    }

    public static ChainBuilder PathfindAndMoveTo(this ChainBuilder builder, VNavmesh vnav, Vector3 destination)
    {
        return builder
            .Debug($"Moving to {destination}")
            .ThenOnFrameworkThread(() => vnav.PathfindAndMoveTo!(destination, false));
    }

    public static ChainBuilder WaitForPathfindingCycle(this ChainBuilder builder, VNavmesh vnav)
    {
        return builder
            .WaitToStartPathfinding(vnav)
            .WaitToStopPathfinding(vnav);
    }

}
