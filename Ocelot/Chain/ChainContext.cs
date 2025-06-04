using System.Threading;

namespace Ocelot.Chain;

public class ChainContext
{
    public CancellationToken token { get; }

    public ChainContext(CancellationToken token)
    {
        this.token = token;
    }
}
