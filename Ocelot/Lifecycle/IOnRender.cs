namespace Ocelot.Lifecycle;

public interface IOnRender : IOrderedHook
{
    void Render();
}
