namespace Ocelot.Lifecycle;

public interface IOnPostUpdate : IOrderedHook
{
    void PostUpdate();
}
