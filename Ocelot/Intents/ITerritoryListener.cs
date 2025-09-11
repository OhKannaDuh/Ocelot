namespace Ocelot.Intents;

[Intent]
public interface ITerritoryListener
{
    void OnTerritoryChanged(ushort id);
}
