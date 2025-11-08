using Ocelot.Services;

namespace Ocelot.Lifecycle;

[OcelotAutoWire]
public interface IOnPostUpdate : IOrderedHook
{
    UpdateLimit UpdateLimit
    {
        get => UpdateLimit.None;
    }

    void PostUpdate();
}
