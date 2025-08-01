using FFXIVClientStructs.FFXIV.Client.Game;

namespace Ocelot.Gameplay;

public partial class Actions
{
    public class Paladin
    {
        public static readonly Action IronWill = new(ActionType.Action, 28);

        public static readonly Action ReleaseIronWill = new(ActionType.Action, 32065);
    }
}
