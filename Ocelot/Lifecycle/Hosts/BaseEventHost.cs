using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public abstract class BaseEventHost(ILogger logger) : IEventHost
{
    private readonly HashSet<string> loggedErrors = [];

    public abstract void Start();

    public abstract void Stop();

    public abstract int Count { get; }

    protected void SafeEach<T>(IEnumerable<T> hooks, Action<T> call)
    {
        foreach (var h in hooks)
        {
            try
            {
                call(h);
            }
            // A hook failing every frame would otherwise write the same error 60 times a second.
            catch (Exception ex) when (!loggedErrors.Add($"{h?.GetType().Name}:{ex.GetType().Name}:{ex.Message}"))
            {
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Lifecycle hook {Hook} failed (logged once per distinct error)", h?.GetType().Name ?? "Unknown");
            }
        }
    }
}
