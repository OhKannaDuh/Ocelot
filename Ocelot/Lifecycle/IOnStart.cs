using Ocelot.Services;

namespace Ocelot.Lifecycle;

[OcelotAutoWire]
public interface IOnStart : IOrderedHook
{
    void OnStart();
}
