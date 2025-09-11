using System.Threading;
using System.Threading.Tasks;

namespace Ocelot.Chain.Steps;

public interface IChainStep
{
    string Name { get; }

    ValueTask<ChainStepResult> ExecuteAsync(ChainContext context, CancellationToken token);
}
