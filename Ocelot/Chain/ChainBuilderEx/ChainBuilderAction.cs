using System;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderAction
{
    public static ChainBuilder WaitGcd(this ChainBuilder builder, int timeout = 3000, int interval = 250)
    {
        return builder
            .Debug($"Waiting for global cooldown")
            .WaitUntil(() =>
        {
            unsafe
            {
                var gcd = ActionManager.Instance()->GetRecastGroupDetail(57);
                return gcd->Total - gcd->Elapsed <= 0;
            }
        }, timeout, interval);
    }

    public static ChainBuilder UseAction(this ChainBuilder builder, ActionType actionType, uint actionId, int timeout = 5000, int interval = 500)
    {
        return builder
            .Debug($"Using action type: {actionType} id: {actionId}")
            .WaitUntil(() =>
            {
                unsafe
                {
                    return ActionManager.Instance()->UseAction(actionType, actionId);
                }
            }, timeout, interval);
    }

    public static ChainBuilder UseGcdAction(this ChainBuilder builder, ActionType actionType, uint actionId, int timeout = 5000, int interval = 500)
    {
        return builder
            .Debug("Using action on global cooldown")
            .WaitGcd(timeout, interval)
            .UseAction(actionType, actionId, timeout, interval);
    }
}
