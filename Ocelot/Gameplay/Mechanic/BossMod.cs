using ECommons.Automation;

namespace Ocelot.Gameplay.Mechanic;

public class BossMod : BaseMechanicPlugin
{
    public override string DisplayName
    {
        get => "Boss Mod";
    }

    public override string InternalName
    {
        get => "BossMod";
    }

    public override string Author
    {
        get => "veyn";
    }

    public override string[] Maintainers
    {
        get => ["xan_0"];
    }

    public override void Enable()
    {
        Chat.ExecuteCommand("/vbmai on");
    }

    public override void Disable()
    {
        Chat.ExecuteCommand("/vbmai off");
    }
}
