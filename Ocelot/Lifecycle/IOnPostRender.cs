namespace Ocelot.Lifecycle;

public interface IOnPostRender : IOrderedHook
{
    void PostRender();
}
