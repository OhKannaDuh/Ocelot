using System.Threading;
using System.Threading.Tasks;
using Ocelot.Chain.Steps;

namespace Ocelot.Chain.Middleware;

public sealed class NextTaskMiddleware : IStepMiddleware
{
    public async ValueTask<ChainStepResult> InvokeAsync(IChainStep step, ChainContext context, CancellationToken token, StepDelegate next)
    {
        context.SetCurrent(step);
        return await next(step, context, token);
    }
}
