using Ocelot.Chain.Steps;
using Ocelot.Internal;

namespace Ocelot.Chain;

public interface IChainStepResultHandler
{
    void HandleResult(IChainStep step, ChainStepResult result, Deque<IChainStep> queue);
}
