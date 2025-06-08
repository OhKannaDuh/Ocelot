using Dalamud.Game.ClientState.Conditions;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using ECommons.Throttlers;

namespace Ocelot.Chain.ChainEx;

public static class ChainCondition
{
    private unsafe static TaskManagerTask WaitUntilCondition(ConditionFlag flag, int timeout = 5000, int interval = 250)
    {
        return new(() =>
        {
            if (EzThrottler.Throttle($"ChainAddon.WaitUntilCondition({flag})", interval))
            {
                return Svc.Condition[flag];
            }

            return false;
        }, new() { TimeLimitMS = timeout });
    }

    public static Chain WaitUntilCondition(this Chain chain, ConditionFlag flag, int timeout = 5000, int interval = 250)
    {
        return chain
            .Debug($"Waiting until player has condition {flag}")
            .Then(WaitUntilCondition(flag, timeout, interval));
    }


    private unsafe static TaskManagerTask WaitUntilNotCondition(ConditionFlag flag, int timeout = 5000, int interval = 250)
    {
        return new(() =>
        {
            if (EzThrottler.Throttle($"ChainAddon.WaitUntilNotCondition({flag})", interval))
            {
                return !Svc.Condition[flag];
            }

            return false;
        }, new() { TimeLimitMS = timeout });
    }

    public static Chain WaitUntilNotCondition(this Chain chain, ConditionFlag flag, int timeout = 5000, int interval = 250)
    {
        return chain
            .Debug($"Waiting until player does not have condition {flag}")
            .Then(WaitUntilNotCondition(flag, timeout, interval));
    }



    public static Chain WaitToCycleCondition(this Chain chain, ConditionFlag flag, int timeout = 5000, int interval = 250)
    {
        return chain.WaitUntilCondition(flag, timeout, interval).WaitUntilNotCondition(flag, timeout, interval);
    }
}
