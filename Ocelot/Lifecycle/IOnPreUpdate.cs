namespace Ocelot.Lifecycle;

public interface IOnPreUpdate : IOrderedHook
{
    UpdateLimit UpdateLimit
    {
        get => UpdateLimit.None;
    }
    
    void PreUpdate();
}
