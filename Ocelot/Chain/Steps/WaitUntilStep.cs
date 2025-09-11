using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ocelot.Chain.Steps;

public class WaitUntilStep(string name, Func<ChainContext, CancellationToken, bool> callback) : IChainStep
{
    public string Name {
        get => name;
    }

    public ValueTask<ChainStepResult> ExecuteAsync(ChainContext context, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return ValueTask.FromResult(new ChainStepResult(ChainStepStatus.Abort));
        }

        if (callback(context, token))
        {
            return ValueTask.FromResult(new ChainStepResult(ChainStepStatus.Continue));
        }

        return ValueTask.FromResult(new ChainStepResult(ChainStepStatus.Requeue));
    }
}
