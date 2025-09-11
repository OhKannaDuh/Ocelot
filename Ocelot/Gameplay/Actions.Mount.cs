using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace Ocelot.Gameplay;

public partial class Actions
{
    public static uint MountId { get; private set; } = 0;

    public static void SetMountId(uint mountId)
    {
        MountId = mountId;
    }

    public static Action Mount()
    {
        return Mount(MountId);
    }

    public static Action Mount(uint id)
    {
        return new Action(ActionType.Mount, id);
    }

    public static Action Unmount {
        get => Dismount;
    }

    public static void TryUnmount()
    {
        if (Svc.Condition[ConditionFlag.Mounted])
        {
            Unmount.Cast();
        }
    }
}
