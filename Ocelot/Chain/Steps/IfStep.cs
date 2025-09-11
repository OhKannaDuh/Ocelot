using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ocelot.Chain.Steps;

public class IfStep(string name, Func<ChainContext, bool> condition, IReadOnlyList<IChainStep> thenSteps, IReadOnlyList<IChainStep>? elseSteps = null) : IChainStep
{
    public string Name { get; } = name;

    public ValueTask<ChainStepResult> ExecuteAsync(ChainContext context, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return ValueTask.FromResult(new ChainStepResult(ChainStepStatus.Abort));
        }

        return ValueTask.FromResult(new ChainStepResult(ChainStepStatus.InsertSteps, condition(context) ? thenSteps : elseSteps));
    }
}
