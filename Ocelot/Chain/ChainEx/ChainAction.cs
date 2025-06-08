using ECommons.Automation.NeoTaskManager;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace Ocelot.Chain.ChainEx;

public static class ChainAction
{
    private unsafe static TaskManagerTask WaitGcd(int timeout = 3000, int interval = 50)
    {
        return new(() =>
        {
            if (EzThrottler.Throttle("ChainAction.WaitGcd", interval))
            {
                var gcd = ActionManager.Instance()->GetRecastGroupDetail(57);
                return gcd->Total - gcd->Elapsed <= 0;
            }

            return false;
        }, new() { TimeLimitMS = timeout });
    }

    public static Chain WaitGcd(this Chain chain, int timeout = 3000, int interval = 50)
    {
        return chain
            .Debug("Waiting for GCD")
            .Then(WaitGcd(timeout, interval));
    }

    private unsafe static TaskManagerTask UseAction(ActionType actionType, uint actionId, int timeout = 3000, int interval = 500)
    {
        return new(() =>
        {
            if (EzThrottler.Throttle($"ChainAction.UseAction({actionType}, {actionId})", interval))
            {
                return ActionManager.Instance()->UseAction(actionType, actionId);
            }

            return false;
        }, new() { TimeLimitMS = timeout });
    }

    public static Chain UseAction(this Chain chain, ActionType actionType, uint actionId, int timeout = 3000, int interval = 500)
    {
        return chain
            .Debug($"Using action ({actionType}, {actionId})")
            .Then(UseAction(actionType, actionId, timeout, interval));
    }

    public static Chain UseGcdAction(this Chain chain, ActionType actionType, uint actionId, int timeout = 3000, int interval = 500)
    {
        return chain.WaitGcd().UseAction(actionType, actionId, timeout, interval);
    }
}
