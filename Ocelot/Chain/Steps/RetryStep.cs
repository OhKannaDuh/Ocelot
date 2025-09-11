using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Ocelot.Chain.Steps;

public abstract class RetryStep(string name) : IChainStep
{
    public string Name { get; } = name;

    protected virtual uint Attempt { get; private set; } = 0;

    protected virtual uint MaxAttempts {
        get => uint.MaxValue;
    }

    protected virtual TimeSpan GetDelayForAttempt(uint attempt)
    {
        return TimeSpan.Zero;
    }

    private long? nextAttempt;

    protected abstract ValueTask<bool> Action(ChainContext context, CancellationToken token);

    protected virtual ChainStepStatus GetExceededAttemptsStatus()
    {
        return ChainStepStatus.Abort;
    }

    public async ValueTask<ChainStepResult> ExecuteAsync(ChainContext context, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return new ChainStepResult(ChainStepStatus.Abort);
        }

        if (nextAttempt is { } deadline)
        {
            if (Stopwatch.GetTimestamp() < deadline)
            {
                return new ChainStepResult(ChainStepStatus.Requeue);
            }

            nextAttempt = null;
        }


        var result = await Action(context, token);
        Attempt++;

        if (result)
        {
            return new ChainStepResult(ChainStepStatus.Continue);
        }

        if (Attempt >= MaxAttempts)
        {
            return new ChainStepResult(GetExceededAttemptsStatus());
        }

        var delay = GetDelayForAttempt(Attempt + 1);
        if (delay > TimeSpan.Zero)
        {
            var add = (long)(delay.TotalSeconds * Stopwatch.Frequency);
            nextAttempt = Stopwatch.GetTimestamp() + add;
        }

        return new ChainStepResult(ChainStepStatus.Requeue);
    }
}
