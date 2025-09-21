namespace Ocelot.Chain;

public delegate Task<StepResult> StepMiddlewareDelegate();

public interface IStepMiddleware
{
    Task<StepResult> InvokeAsync(IChainContext context, IStep step, StepMiddlewareDelegate next);
}
