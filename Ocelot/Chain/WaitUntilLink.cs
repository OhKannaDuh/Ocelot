using System;
using System.Threading.Tasks;

namespace Ocelot.Chain;

public class WaitUntilLink : IChainlink
{
    private readonly Func<bool> predicate;

    private readonly int timeout;

    private readonly int interval;

    public WaitUntilLink(Func<bool> predicate, int timeout = 5000, int interval = 250)
    {
        this.predicate = predicate;
        this.timeout = timeout;
        this.interval = interval;
    }

    public async Task RunAsync(ChainContext context)
    {
        var waited = 0;
        while (!predicate() && waited < timeout && !context.token.IsCancellationRequested)
        {
            await Task.Delay(interval, context.token);
            waited += interval;
        }
    }
}
