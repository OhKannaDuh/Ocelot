namespace Ocelot.Chain.Steps;

public class ChainStep(IChain chain) : IStep
{
    public async Task<StepResult> ExecuteAsync(IChainContext context)
    {
        var result = await chain.ExecuteAsync(context);

        if (result.IsCanceled || context.CancellationToken.IsCancellationRequested)
        {
            return StepResult.Canceled();
        }

        return result.IsSuccess
            ? StepResult.Success()
            : StepResult.Failure(result.ErrorMessage ?? "Unknown error");
    }
}
