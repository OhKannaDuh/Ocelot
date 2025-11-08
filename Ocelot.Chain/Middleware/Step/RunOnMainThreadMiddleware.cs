using Ocelot.Chain.Thread;

namespace Ocelot.Chain.Middleware.Step;

public class RunOnMainThreadMiddleware(IMainThread thread) : IStepMiddleware
{
    public Task<StepResult> InvokeAsync(IChainContext context, IStep step, StepMiddlewareDelegate next)
    {
        if (thread.IsMainThread)
        {
            return next();
        }

        return thread.InvokeAsync(() => next(), context.CancellationToken);
    }
}
