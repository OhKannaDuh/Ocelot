using FFXIVClientStructs.FFXIV.Client.Game;

namespace Ocelot.Gameplay;

public partial class Actions
{
    protected Actions()
    {
    }

    public static Action Sprint { get; private set; } = new(ActionType.GeneralAction, 4);

    public static Action Return { get; private set; } = new(ActionType.GeneralAction, 8);
}
