namespace Ocelot.Lifecycle;

public interface IOrderedHook
{
    // Highest first
    // Stop runs in reverse order
    int Order
    {
        get => 0;
    }
}
