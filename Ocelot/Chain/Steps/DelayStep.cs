using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Ocelot.Chain.Steps;

public sealed class DelayStep(string name, TimeSpan delay) : IChainStep
{
    public string Name { get; } = name;


    private long? ticks;

    public ValueTask<ChainStepResult> ExecuteAsync(ChainContext context, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return ValueTask.FromResult(new ChainStepResult(ChainStepStatus.Abort));
        }

        if (delay <= TimeSpan.Zero)
        {
            return ValueTask.FromResult(new ChainStepResult(ChainStepStatus.Continue));
        }

        if (ticks is null)
        {
            var add = (long)(delay.TotalSeconds * Stopwatch.Frequency);
            ticks = Stopwatch.GetTimestamp() + add;
        }

        var now = Stopwatch.GetTimestamp();
        var ready = now >= ticks.Value;

        return ValueTask.FromResult(new ChainStepResult(ready ? ChainStepStatus.Continue : ChainStepStatus.Requeue));
    }
}
