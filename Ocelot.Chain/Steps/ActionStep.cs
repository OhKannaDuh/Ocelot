namespace Ocelot.Chain.Steps;

public class ActionStep(Func<IChainContext, ValueTask<StepResult>> action, string name) : IStep
{
    public Task<StepResult> ExecuteAsync(IChainContext context)
    {
        return action(context).AsTask();
    }

    public override string ToString()
    {
        return name;
    }
}
