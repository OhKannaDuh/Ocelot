using FFXIVClientStructs.FFXIV.Client.Game;

namespace Ocelot.Gameplay;

public partial class Actions
{
    public class DarkKnight
    {
        public static readonly Action Grit = new(ActionType.Action, 3629);

        public static readonly Action ReleaseGrit = new(ActionType.Action, 32067);
    }
}
