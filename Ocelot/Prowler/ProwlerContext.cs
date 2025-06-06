using System.Threading;

namespace Ocelot.Prowler;

public class ProwlerContext
{
    public CancellationTokenSource source { get; }

    public CancellationToken token => source.Token;

    public ProwlerContext(CancellationTokenSource source)
    {
        this.source = source;
    }
}
