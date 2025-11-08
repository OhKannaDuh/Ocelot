using Ocelot.Services;

namespace Ocelot.Lifecycle;

[OcelotAutoWire]
public interface IOnStop : IOrderedHook
{
    void OnStop();
}
