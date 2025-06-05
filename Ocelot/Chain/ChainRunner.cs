using System.Threading;
using System.Threading.Tasks;

namespace Ocelot.Chain;


public static class ChainRunner
{
    public static async Task Run(IChainlink task)
    {
        var source = new CancellationTokenSource();
        var context = new ChainContext(source);

        await task.RunAsync(context);
    }
}
