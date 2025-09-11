namespace Ocelot.Gameplay.Rotation;

public class BlankRotationPlugin(OcelotPlugin _) : BaseRotationPlugin
{
    public override string DisplayName {
        get => "None";
    }

    public override string InternalName {
        get => "None";
    }

    public override string Author {
        get => "N/A";
    }

    public override void Enable() { }

    public override void Disable() { }
}
