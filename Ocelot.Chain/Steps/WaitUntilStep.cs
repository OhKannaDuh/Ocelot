namespace Ocelot.Chain.Steps;

public class WaitUntilStep(
    Func<IChainContext, ValueTask<bool>> predicate,
    TimeSpan timeout,
    TimeSpan? pollInterval = null,
    string? name = null
) : IStep
{
    private readonly Func<IChainContext, ValueTask<bool>> predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

    private readonly TimeSpan timeout = timeout < TimeSpan.Zero ? TimeSpan.Zero : timeout;

    private readonly TimeSpan poll =
        ((pollInterval ?? TimeSpan.FromMilliseconds(250)) <= TimeSpan.Zero)
            ? TimeSpan.FromMilliseconds(1)
            : (pollInterval ?? TimeSpan.FromMilliseconds(250));

    public override string ToString()
    {
        return name ?? nameof(WaitUntilStep);
    }

    public async Task<StepResult> ExecuteAsync(IChainContext context)
    {
        DateTimeOffset? deadline = null;
        if (timeout > TimeSpan.Zero)
        {
            var now = DateTimeOffset.UtcNow;
            var maxAdd = DateTimeOffset.MaxValue - now;
            deadline = timeout >= maxAdd ? DateTimeOffset.MaxValue : now + timeout;
        }
        
        while (true)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (await predicate(context))
            {
                return StepResult.Success();
            }

            if (deadline is { } d && DateTimeOffset.UtcNow >= d)
            {
                return StepResult.Failure($"WaitUntil timed out after {timeout}.");
            }

            await Task.Delay(poll, context.CancellationToken);
        }
    }
}
