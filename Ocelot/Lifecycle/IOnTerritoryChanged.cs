namespace Ocelot.Lifecycle;

public interface IOnTerritoryChanged : IOrderedHook
{
    void IOnTerritoryChanged(ushort territory);
}
