using System;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace Ocelot.Gameplay;

public unsafe class Action(ActionType type, uint id)
{
    public float GetRecastTime()
    {
        var manager = ActionManager.Instance();
        var recast = manager->GetRecastTime(type, id);
        var elapsed = manager->GetRecastTimeElapsed(type, id);

        return recast - elapsed;
    }

    public unsafe bool CanCast()
    {
        return GetRecastTime() <= 0f && ActionManager.Instance()->GetActionStatus(type, id) <= 0f;
    }

    public void Cast()
    {
        ActionManager.Instance()->UseAction(type, id);
    }

    public Func<Chain.Chain> GetCastChain()
    {
        return () => CastOnChain(Chain.Chain.Create($"Action({type}, {id})"));
    }

    public Chain.Chain CastOnChain(Chain.Chain chain)
    {
        return chain
            .Debug("Waiting to be able to cast")
            .Then(_ => CanCast())
            .Debug("Casting")
            .Then(_ => Cast());
    }
}
