using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ocelot.Chain;

public class Chain : List<IChainlink>, IChainlink
{
    public async Task RunAsync(ChainContext context)
    {
        foreach (var task in this)
        {
            if (context.token.IsCancellationRequested)
                break;

            await task.RunAsync(context);
        }
    }
}
