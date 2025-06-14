using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using ECommons.Throttlers;

namespace Ocelot.Chain.ChainEx;

public static class ChainStatus
{
    private unsafe static TaskManagerTask WaitUntilStatus(uint status, int timeout = 5000, int interval = 50)
    {
        return new(() =>
        {
            if (EzThrottler.Throttle($"ChainStatus.WaitUntilStatus({status})", interval))
            {
                return Svc.ClientState.LocalPlayer?.StatusList.Any(s => s.StatusId == status) == true;
            }

            return false;
        }, new() { TimeLimitMS = timeout });
    }

    public static Chain WaitUntilStatus(this Chain chain, uint status, int timeout = 5000, int interval = 50)
    {
        return chain
            .Debug($"Waiting until player has status {status}")
            .Then(WaitUntilStatus(status, timeout, interval));
    }

    private unsafe static TaskManagerTask WaitUntilNotStatus(uint status, int timeout = 5000, int interval = 50)
    {
        return new(() =>
        {
            if (EzThrottler.Throttle($"ChainStatus.WaitUntilNotStatus({status})", interval))
            {
                return Svc.ClientState.LocalPlayer?.StatusList.Any(s => s.StatusId == status) == false;
            }

            return false;
        }, new() { TimeLimitMS = timeout });
    }

    public static Chain WaitUntilNotStatus(this Chain chain, uint status, int timeout = 5000, int interval = 50)
    {
        return chain
            .Debug($"Waiting until player does not have status {status}")
            .Then(WaitUntilNotStatus(status, timeout, interval));
    }

    public static Chain WaitToCycleCondition(this Chain chain, uint status, int timeout = 5000, int interval = 50)
    {
        return chain.WaitUntilNotStatus(status, timeout, interval).WaitUntilNotStatus(status, timeout, interval);
    }
}
