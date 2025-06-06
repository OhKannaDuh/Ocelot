using System;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Enums;
using ECommons.DalamudServices;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderStatus
{
    public static ChainBuilder WaitUntilStatus(this ChainBuilder builder, StatusFlags status, int timeout = 5000, int interval = 250)
    {
        return builder
            .Debug($"Waiting until status {status} is active")
            .WaitOnFrameworkThreadUntil(() =>
            {
                var player = Svc.ClientState.LocalPlayer;
                if (player == null)
                {
                    return false;
                }

                return player.StatusFlags.HasFlag(status);
            }, timeout, interval);
    }

    public static ChainBuilder WaitWhileStatus(this ChainBuilder builder, StatusFlags status, int timeout = 5000, int interval = 250)
    {
        return builder
            .Debug($"Waiting while status {status} is active")
            .WaitOnFrameworkThreadWhile(() =>
            {
                var player = Svc.ClientState.LocalPlayer;
                if (player == null)
                {
                    return false;
                }

                var hasStatus = player.StatusFlags.HasFlag(status);
                builder.Debug($"Player has status {status}: {hasStatus}");
                return hasStatus;
            }, timeout, interval);
    }
}
