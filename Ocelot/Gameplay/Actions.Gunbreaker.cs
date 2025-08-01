using FFXIVClientStructs.FFXIV.Client.Game;

namespace Ocelot.Gameplay;

public partial class Actions
{
    public class Gunbreaker
    {
        public static readonly Action RoyalGuard = new(ActionType.Action, 16142);

        public static readonly Action ReleaseRoyalGuard = new(ActionType.Action, 32068);
    }
}
