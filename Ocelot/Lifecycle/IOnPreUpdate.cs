namespace Ocelot.Lifecycle;

public interface IOnPreUpdate : IOrderedHook
{
    void PreUpdate();
}
