using ECommons.Automation;

namespace Ocelot.Gameplay.Mechanic;

public class BossModReborn : BaseMechanicPlugin
{
    public override string DisplayName
    {
        get => "Bossmod Reborn";
    }

    public override string InternalName
    {
        get => "BossModReborn";
    }

    public override string Author
    {
        get => "The Combat Reborn Team";
    }

    public override void Enable()
    {
        Chat.ExecuteCommand("/bmrai on");
    }

    public override void Disable()
    {
        Chat.ExecuteCommand("/bmrai off");
    }
}
