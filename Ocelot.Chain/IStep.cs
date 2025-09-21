namespace Ocelot.Chain;

public interface IStep
{
    Task<StepResult> ExecuteAsync(IChainContext context);

    IEnumerable<IStepMiddleware> GetMiddleware()
    {
        return [];
    }
}
