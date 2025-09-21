namespace Ocelot.Lifecycle;

public interface IOnPostUpdate : IOrderedHook
{
    UpdateLimit UpdateLimit
    {
        get => UpdateLimit.None;
    }
    
    void PostUpdate();
}
