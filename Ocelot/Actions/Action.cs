using FFXIVClientStructs.FFXIV.Client.Game;

namespace Ocelot.Actions;

public unsafe class Action(ActionType type, uint id)
{
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
}
