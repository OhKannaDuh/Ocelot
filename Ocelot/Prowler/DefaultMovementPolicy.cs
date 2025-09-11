using System;
using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using Ocelot.Services;

namespace Ocelot.Prowler;

[OcelotService(typeof(IMovementPolicy))]
public class DefaultMovementPolicy : IMovementPolicy
{
    public Func<ProwlContext, bool> Fly { get; init; } = _ => !Svc.Condition[ConditionFlag.InCombat] && Player.CanMount && Player.CanFly;

    public Func<ProwlContext, bool> Mount { get; init; } = _ => !Svc.Condition[ConditionFlag.InCombat] && Player.CanMount;

    public Func<ProwlContext, bool> Sprint { get; init; } = _ => true;


    public bool ShouldFly(in ProwlContext ctx)
    {
        return Fly(ctx);
    }

    public bool ShouldMount(in ProwlContext ctx)
    {
        return Mount(ctx);
    }

    public bool ShouldSprint(in ProwlContext ctx)
    {
        return Sprint(ctx);
    }
}
