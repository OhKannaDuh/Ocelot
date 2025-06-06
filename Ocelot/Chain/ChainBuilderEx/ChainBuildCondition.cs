using ECommons.DalamudServices;
using Dalamud.Game.ClientState.Conditions;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuildCondition
{
    public static ChainBuilder WaitUntilCondition(this ChainBuilder builder, ConditionFlag flag, int timeout = 5000, int interval = 250)
    {
        return builder
            .Debug($"Waiting until condition {flag} is active")
            .WaitOnFrameworkThreadUntil(() => Svc.Condition[flag], timeout, interval);
    }

    public static ChainBuilder WaitWhileCondition(this ChainBuilder builder, ConditionFlag flag, int timeout = 5000, int interval = 250)
    {
        return builder
            .Debug($"Waiting while condition {flag} is active")
            .WaitOnFrameworkThreadWhile(() => Svc.Condition[flag], timeout, interval);
    }

    public static ChainBuilder WaitForConditionCycle(this ChainBuilder builder, ConditionFlag flag, int timeout = 5000, int interval = 250)
    {
        return builder
            .Debug($"Waiting for condition {flag} to cycle")
            .WaitUntilCondition(flag, timeout, interval)
            .WaitWhileCondition(flag, timeout, interval);
    }
}
