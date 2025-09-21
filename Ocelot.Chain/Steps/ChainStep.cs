namespace Ocelot.Chain.Steps;

public class ChainStep(IChain chain) : IStep
{
    public async Task<StepResult> ExecuteAsync(IChainContext context)
    {
        var result = await chain.ExecuteAsync();
        return !result.IsSuccess ? StepResult.Failure(result.ErrorMessage ?? "Unknown error") : StepResult.Success();
    }
}
