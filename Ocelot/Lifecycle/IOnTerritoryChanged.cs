using Ocelot.Services;

namespace Ocelot.Lifecycle;

[OcelotAutoWire]
public interface IOnTerritoryChanged : IOrderedHook
{
    void IOnTerritoryChanged(ushort territory);
}
