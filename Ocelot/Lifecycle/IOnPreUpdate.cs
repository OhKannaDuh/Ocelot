using Ocelot.Services;

namespace Ocelot.Lifecycle;

[OcelotAutoWire]
public interface IOnPreUpdate : IOrderedHook
{
    UpdateLimit UpdateLimit
    {
        get => UpdateLimit.None;
    }

    void PreUpdate();
}
