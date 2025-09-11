using System.Threading;
using System.Threading.Tasks;

namespace Ocelot.Chain.Steps;

public abstract class ActionStep(string name) : IChainStep
{
    public string Name { get; } = name;

    protected abstract ValueTask<bool> Action(ChainContext context, CancellationToken token);

    public async ValueTask<ChainStepResult> ExecuteAsync(ChainContext context, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return new ChainStepResult(ChainStepStatus.Abort);
        }

        await Action(context, token);

        return new ChainStepResult(ChainStepStatus.Continue);
    }
}
