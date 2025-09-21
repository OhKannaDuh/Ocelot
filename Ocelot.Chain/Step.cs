namespace Ocelot.Chain;

public class Step : IStep
{
    public Task<StepResult> ExecuteAsync(IChainContext context)
    {
        return Task.FromResult(StepResult.Success());
    }
}
