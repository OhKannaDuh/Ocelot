using ECommons.DalamudServices;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderPlayer
{
    public static ChainBuilder WaitUntilCasting(this ChainBuilder builder, int timeout = 5000, int interval = 250)
    {
        return builder
            .Debug("Waiting for player to start casting")
            .WaitOnFrameworkThreadUntil(() =>
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
            .WaitOnFrameworkThreadWhile(() =>
            {
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
            .WaitOnFrameworkThreadUntil(() =>
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
            .WaitOnFrameworkThreadWhile(() =>
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
