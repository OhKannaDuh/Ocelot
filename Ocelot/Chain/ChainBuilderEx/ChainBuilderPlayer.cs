using System;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.UI.Arrays;

namespace Ocelot.Chain.ChainBuilderEx;

public class CastInteruptedException : Exception { }

public static class ChainBuilderPlayer
{
    public static ChainBuilder WaitUntilCasting(this ChainBuilder builder, int timeout = 5000, int interval = 250)
    {
        return builder
            .Debug("Waiting for player to start casting")
            .WaitUntil(() =>
            {
                var player = Svc.ClientState.LocalPlayer;
                if (player == null) return false;

                return player.IsCasting;
            }, timeout, interval);
    }

    public static ChainBuilder WaitWhileCasting(this ChainBuilder builder, int timeout = 5000, int interval = 250)
    {
        return builder
            .Debug("Waiting for player to stop casting")
            .WaitWhile(() =>
            {
                unsafe
                {
                    var cast = CastBarNumberArray.Instance();
                    if (cast->Interupted)
                    {
                        throw new CastInteruptedException();
                    }
                }

                var player = Svc.ClientState.LocalPlayer;
                if (player == null) return false;

                return player.IsCasting;
            }, timeout, interval);
    }

    public static ChainBuilder WaitCastingCycle(this ChainBuilder builder, int timeout = 5000, int interval = 250)
    {
        return builder
            .Debug("Waiting for player casting cycle")
            .WaitUntilCasting(timeout, interval)
            .WaitWhileCasting(timeout, interval);
    }

    public static ChainBuilder WaitUntilDead(this ChainBuilder builder, int timeout = 5000, int interval = 250)
    {
        return builder
            .Debug("Waiting for player to be dead")
            .WaitUntil(() =>
            {
                var player = Svc.ClientState.LocalPlayer;
                if (player == null) return false;

                return player.IsDead;
            }, timeout, interval);
    }

    public static ChainBuilder WaitWhileDead(this ChainBuilder builder, int timeout = 5000, int interval = 250)
    {
        return builder
            .Debug("Waiting for player to be alive")
            .WaitWhile(() =>
            {
                var player = Svc.ClientState.LocalPlayer;
                if (player == null) return false;

                return player.IsDead;
            }, timeout, interval);
    }

    public static ChainBuilder WaitDeadCycle(this ChainBuilder builder, int timeout = 5000, int interval = 250)
    {
        return builder
            .Debug("Waiting for player death cycle")
            .WaitUntilDead(timeout, interval)
            .WaitWhileDead(timeout, interval);
    }
}
