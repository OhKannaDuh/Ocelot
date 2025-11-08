using FFXIVClientStructs.FFXIV.Client.Game;
using Ocelot.Services.PlayerState;

namespace Ocelot.Actions;

public partial class Actions
{
    private static uint MountId = 0;

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

    public static Action Unmount
    {
        get => Dismount;
    }

    public static void TryUnmount(IPlayer player)
    {
        if (player.IsMounted())
        {
            Unmount.Cast();
        }
    }
}
