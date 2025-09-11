using System.Threading;
using System.Threading.Tasks;
using Ocelot.Chain.Steps;

namespace Ocelot.Chain.Middleware;

public interface IStepMiddleware
{
    ValueTask<ChainStepResult> InvokeAsync(IChainStep step, ChainContext context, CancellationToken token, StepDelegate next);
}
