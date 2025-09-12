namespace Ocelot.Lifecycle;

public interface IOnUpdate : IOrderedHook
{
    void Update();
}
