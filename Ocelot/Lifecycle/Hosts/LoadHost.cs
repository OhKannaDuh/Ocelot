using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public class LoadHost(IEnumerable<IOnLoad> load, ILogger logger) : BaseEventHost(logger), IOrderedHook
{
    private readonly IOnLoad[] load = load.OrderByDescending(h => h.Order).ToArray();

    public int Order
    {
        get => int.MaxValue;
    }

    public override int Count
    {
        get => load.Length;
    }

    public override void Start()
    {
        foreach (var hook in load)
        {
            try
            {
                hook.OnLoad();
            }
            catch (Exception ex)
            {
                // Logger not initialized on load
            }
        }
    }

    public override void Stop()
    {
    }
}
