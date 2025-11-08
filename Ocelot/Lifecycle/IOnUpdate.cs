using Ocelot.Services;

namespace Ocelot.Lifecycle;

[OcelotAutoWire]
public interface IOnUpdate : IOrderedHook
{
    UpdateLimit UpdateLimit
    {
        get => UpdateLimit.None;
    }

    void Update();
}
