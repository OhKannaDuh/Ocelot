using Ocelot.Services;

namespace Ocelot.Lifecycle;

[OcelotAutoWire]
public interface IOnPostRender : IOrderedHook
{
    void PostRender();
}
