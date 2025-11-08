using Ocelot.Services;

namespace Ocelot.Lifecycle;

[OcelotAutoWire]
public interface IOnRender : IOrderedHook
{
    void Render();
}
