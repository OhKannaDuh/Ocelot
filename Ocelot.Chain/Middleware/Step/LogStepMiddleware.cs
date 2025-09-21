using System.Diagnostics;
using Ocelot.Services.Logger;

namespace Ocelot.Chain.Middleware.Step;

public class LogStepMiddleware(ILogger logger) : IStepMiddleware
{
    public async Task<StepResult> InvokeAsync(IChainContext context, IStep step, StepMiddlewareDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        logger.Info("Step {Step} started (RunId={RunId})", step.GetType().Name, context.RunId);

        try
        {
            var result = await next();
            stopwatch.Stop();

            if (result.IsSuccess)
            {
                logger.Info("Step {Step} succeeded in {Elapsed} ms", step.GetType().Name, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logger.Warning("Step {Step} failed in {Elapsed} ms: {Error}", step.GetType().Name, stopwatch.ElapsedMilliseconds,
                    result.ErrorMessage ?? "Unknown Error");
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            logger.Warning("Step {Step} canceled after {Elapsed} ms", step.GetType().Name, stopwatch.ElapsedMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.Error("Step {Step} threw after {Elapsed} ms. Error: {Error}", step.GetType().Name, stopwatch.ElapsedMilliseconds, ex.Message);
            if (ex.StackTrace != null)
            {
                logger.Error(ex.StackTrace);
            }

            throw;
        }
    }
}
