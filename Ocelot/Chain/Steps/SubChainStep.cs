using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ocelot.Chain.Steps;

public class SubChainStep(string name, Func<ChainRunner> factory) : IChainStep, IDisposable
{
    private ChainRunner? runner;

    public string Name { get; } = name;

    public ValueTask<ChainStepResult> ExecuteAsync(ChainContext context, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            runner?.Dispose();
            runner = null;
            return ValueTask.FromResult(new ChainStepResult(ChainStepStatus.Abort));
        }

        runner ??= factory();

        try
        {
            runner.Tick();
        }
        catch (Exception ex)
        {
            return ValueTask.FromResult(new ChainStepResult(ChainStepStatus.Fail, Error: ex));
        }

        var done = runner.IsCompleted;
        return ValueTask.FromResult(new ChainStepResult(done ? ChainStepStatus.Continue : ChainStepStatus.Requeue));
    }

    public void Dispose()
    {
        runner?.Dispose();
    }
}
