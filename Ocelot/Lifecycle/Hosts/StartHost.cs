using System.Collections.Generic;
using System.Linq;
using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public class StartHost(IEnumerable<IOnStart> start, ILoggerService logger) : BaseEventHost(logger), IOrderedHook
{
    private readonly IOnStart[] start = start.OrderByDescending(h => h.Order).ToArray();

    public int Order
    {
        get => int.MaxValue;
    }

    public override int Count
    {
        get => start.Length;
    }

    public override void Start()
    {
        SafeEach(start, h => h.OnStart());
    }

    public override void Stop()
    {
    }
}
