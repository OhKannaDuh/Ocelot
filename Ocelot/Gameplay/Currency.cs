namespace Ocelot.Gameplay;

public class Currency(uint id, uint max = 0) : Item(id, max)
{
    public override void Use() { }
}
