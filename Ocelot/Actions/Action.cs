using FFXIVClientStructs.FFXIV.Client.Game;

namespace Ocelot.Actions;

public unsafe class Action(ActionType type, uint id)
{
    public ActionType Type = type;

    public uint Id = id;

    public float GetRecastTime()
    {
        var manager = ActionManager.Instance();
        var recast = manager->GetRecastTime(Type, Id);
        var elapsed = manager->GetRecastTimeElapsed(Type, Id);

        return recast - elapsed;
    }

    public bool CanCast()
    {
        return GetRecastTime() <= 0f && ActionManager.Instance()->GetActionStatus(Type, Id) <= 0f;
    }

    public bool Cast() => Cast(0xE0000000);

    public bool Cast(ulong targetId)
    {
        using (ActionCastScope.SuppressPathfindCancel())
        {
            return ActionManager.Instance()->UseAction(Type, Id, targetId);
        }
    }

    public bool IsValid()
    {
        return Id != 0 && Type != ActionType.None;
    }
}
