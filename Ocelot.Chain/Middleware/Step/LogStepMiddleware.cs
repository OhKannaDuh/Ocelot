using System.Diagnostics;
using Ocelot.Services.Logger;

namespace Ocelot.Chain.Middleware.Step;

public class LogStepMiddleware(ILogger logger) : IStepMiddleware
{
    public async Task<StepResult> InvokeAsync(IChainContext context, IStep step, StepMiddlewareDelegate next)
    {
        var name = step.ToString() ?? step.GetType().Name;
        var stopwatch = Stopwatch.StartNew();
        logger.Verbose("Step {Step} started (RunId={RunId})", name, context.RunId);

        try
        {
            var result = await next();
            stopwatch.Stop();

            if (result.IsSuccess)
            {
                logger.Verbose("Step {Step} succeeded in {Elapsed} ms", name, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logger.Warning("Step {Step} failed in {Elapsed} ms: {Error}", name, stopwatch.ElapsedMilliseconds,
                    result.ErrorMessage ?? "Unknown Error");
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            logger.Debug("Step {Step} canceled after {Elapsed} ms", name, stopwatch.ElapsedMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.Error("Step {Step} threw after {Elapsed} ms. Error: {Error}", name, stopwatch.ElapsedMilliseconds, ex.Message);
            if (ex.StackTrace != null)
            {
                logger.Error(ex.StackTrace);
            }

            throw;
        }
    }
}
