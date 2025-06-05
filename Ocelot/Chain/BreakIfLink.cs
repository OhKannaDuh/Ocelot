using System;
using System.Threading.Tasks;

namespace Ocelot.Chain;

public class BreakIfLink : IChainlink
{
    private readonly Func<bool> condition;

    public BreakIfLink(Func<bool> condition)
    {
        this.condition = condition;
    }

    public Task RunAsync(ChainContext context)
    {
        if (condition())
        {
            context.source.Cancel();
        }

        return Task.CompletedTask;
    }
}
