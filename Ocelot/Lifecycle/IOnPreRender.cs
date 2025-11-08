using Ocelot.Services;

namespace Ocelot.Lifecycle;

[OcelotAutoWire]
public interface IOnPreRender : IOrderedHook
{
    void PreRender();
}
