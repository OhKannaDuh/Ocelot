using System;
using System.Threading.Tasks;

namespace Ocelot.Chain;

public class DelayLink : IChainlink
{
    private readonly int milliseconds;

    public DelayLink(int milliseconds)
    {
        this.milliseconds = milliseconds;
    }

    public DelayLink(TimeSpan time)
    {
        milliseconds = time.Milliseconds;
    }

    public async Task RunAsync(ChainContext context)
    {
        await Task.Delay(milliseconds, context.token);
    }
}
