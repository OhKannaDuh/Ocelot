using System;
using System.Threading;
using System.Threading.Tasks;
using Ocelot.Chain.Steps;

namespace Ocelot.Chain.Middleware;

public class TaskHookMiddleware : IStepMiddleware
{
    public async ValueTask<ChainStepResult> InvokeAsync(IChainStep step, ChainContext context, CancellationToken ct, StepDelegate next)
    {
        var started = DateTime.UtcNow;
        try
        {
            var outcome = await next(step, context, ct);
            context.OnStepCompleted?.Invoke(step.Name, DateTime.UtcNow - started);
            return outcome;
        }
        catch (Exception ex)
        {
            context.OnStepFailed?.Invoke(step.Name, ex);
            throw;
        }
    }
}
