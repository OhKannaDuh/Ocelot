using Ocelot.Lifecycle.Hosts;

namespace Ocelot.Lifecycle;

public interface IOnUpdate : IOrderedHook
{
    UpdateLimit UpdateLimit
    {
        get => UpdateLimit.None;
    }
    
    void Update();
}
