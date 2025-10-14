namespace Ocelot.Lifecycle;

public interface IOnLoad : IOrderedHook
{
    void OnLoad();
}
