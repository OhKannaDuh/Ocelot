namespace Ocelot.Chain.Steps;

public class WaitStep(TimeSpan delay, string? name = null) : IStep
{
    private readonly TimeSpan delay = delay < TimeSpan.Zero ? TimeSpan.Zero : delay;

    public WaitStep(int milliseconds, string? name = null)
        : this(TimeSpan.FromMilliseconds(milliseconds), name)
    {
    }

    public async Task<StepResult> ExecuteAsync(IChainContext context)
    {
        if (delay > TimeSpan.Zero)
        {
            await Task.Delay(delay, context.CancellationToken);
        }

        return StepResult.Success();
    }

    public override string ToString()
    {
        return name ?? nameof(WaitStep);
    }
}
