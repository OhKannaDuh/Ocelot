using Ocelot.Services;

namespace Ocelot.Lifecycle;

[OcelotAutoWire]
public interface IOnLoad : IOrderedHook
{
    void OnLoad();
}
