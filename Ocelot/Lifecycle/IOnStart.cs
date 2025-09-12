namespace Ocelot.Lifecycle;

public interface IOnStart : IOrderedHook
{
    void OnStart();
}
