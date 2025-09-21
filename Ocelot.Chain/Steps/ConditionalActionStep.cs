namespace Ocelot.Chain.Steps;

internal sealed class ConditionalActionStep(
    Func<IChainContext, ValueTask<bool>> condition,
    Func<IChainContext, ValueTask<StepResult>> thenAction,
    Func<IChainContext, ValueTask<StepResult>>? elseAction = null,
    string? name = null)
    : IStep
{
    private readonly Func<IChainContext, ValueTask<bool>> condition = condition ?? throw new ArgumentNullException(nameof(condition));
    private readonly Func<IChainContext, ValueTask<StepResult>> thenAction = thenAction ?? throw new ArgumentNullException(nameof(thenAction));
    private readonly Func<IChainContext, ValueTask<StepResult>> elseAction = elseAction ?? (_ => new ValueTask<StepResult>(StepResult.Success()));

    public async Task<StepResult> ExecuteAsync(IChainContext context)
    {
        var runThen = await condition(context);
        return runThen ? await thenAction(context) : await elseAction(context);
    }

    public override string ToString()
    {
        return name ?? nameof(ConditionalActionStep);
    }
}
