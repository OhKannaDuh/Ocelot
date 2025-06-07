using ECommons.DalamudServices;
using Dalamud.Game.ClientState.Conditions;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderCondition
{
    public static ChainBuilder WaitUntilCondition(this ChainBuilder builder, ConditionFlag flag, int timeout = 5000, int interval = 250)
        => builder
            .Debug($"Waiting until condition {flag} is active")
            .WaitUntil(() => Svc.Condition[flag], timeout, interval);

    public static ChainBuilder WaitWhileCondition(this ChainBuilder builder, ConditionFlag flag, int timeout = 5000, int interval = 250)
        => builder
            .Debug($"Waiting unitl condition {flag} is not active")
            .WaitWhile(() => Svc.Condition[flag], timeout, interval);

    public static ChainBuilder WaitForConditionCycle(this ChainBuilder builder, ConditionFlag flag, int timeout = 5000, int interval = 250)
        => builder
            .Debug($"Waiting for condition {flag} to cycle")
            .WaitUntilCondition(flag, timeout, interval)
            .WaitWhileCondition(flag, timeout, interval);
}
