using FFXIVClientStructs.FFXIV.Client.Game;
using Ocelot.Chain;
using Ocelot.Chain.Builder;

namespace Ocelot.Gameplay;

public unsafe class Action(ActionType type, uint id)
{
    protected string Key {
        get => $"Action({type}, {id})";
    }

    public float GetRecastTime()
    {
        var manager = ActionManager.Instance();
        var recast = manager->GetRecastTime(type, id);
        var elapsed = manager->GetRecastTimeElapsed(type, id);

        return recast - elapsed;
    }

    public bool CanCast()
    {
        return GetRecastTime() <= 0f && ActionManager.Instance()->GetActionStatus(type, id) <= 0f;
    }

    public void Cast()
    {
        ActionManager.Instance()->UseAction(type, id);
    }

    public ChainRunner GetChainRunner()
    {
        return ChainBuilder.Default(Key)
            .Debug($"Waiting to be able to cast {Key}")
            .WaitUntil($"{Key} can cast", (_, _) => CanCast())
            .Debug($"Casting {Key}")
            .Then($"Cast {Key}", (_, _) => Cast())
            .Build();
    }
}
