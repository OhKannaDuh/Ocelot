using System;
using System.Threading.Tasks;

namespace Ocelot.Chain;

public class ConditionalLink : IChainlink
{
    private readonly Func<bool> condition;
    private readonly IChainlink link;

    public ConditionalLink(Func<bool> condition, IChainlink link)
    {
        this.condition = condition;
        this.link = link;
    }

    public async Task RunAsync(ChainContext context)
    {
        if (condition())
        {
            await link.RunAsync(context);
        }
    }
}
