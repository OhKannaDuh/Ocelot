namespace Ocelot.Gameplay.Mechanic;

public abstract class BaseMechanicPlugin : IMechanicPlugin
{
    public abstract string DisplayName { get; }

    public abstract string InternalName { get; }

    public abstract string Author { get; }

    public virtual string[] Maintainers
    {
        get => [];
    }

    public abstract void Enable();

    public abstract void Disable();

    public virtual void Dispose()
    {
    }
}
