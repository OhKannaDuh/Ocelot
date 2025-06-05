using System.Threading;

namespace Ocelot.Chain;

public class ChainContext
{
    public CancellationTokenSource source { get; }
    public CancellationToken token => source.Token;

    public ChainContext(CancellationTokenSource source)
    {
        this.source = source;
    }
}
