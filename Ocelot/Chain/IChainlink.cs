using System.Threading.Tasks;

namespace Ocelot.Chain;

public interface IChainlink
{
    Task RunAsync(ChainContext context);
}
