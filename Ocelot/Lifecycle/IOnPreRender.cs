namespace Ocelot.Lifecycle;

public interface IOnPreRender : IOrderedHook
{
    void PreRender();
}
