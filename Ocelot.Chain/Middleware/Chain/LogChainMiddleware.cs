using System.Diagnostics;
using Ocelot.Services.Logger;

namespace Ocelot.Chain.Middleware.Chain;

public class LogChainMiddleware(ILogger logger) : IChainMiddleware
{
    public async Task<ChainResult> InvokeAsync(IChainContext context, ChainMiddlewareDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        logger.Info("Chain {Name} started (RunId={RunId})", context.ChainName, context.RunId);

        try
        {
            var result = await next();
            stopwatch.Stop();

            if (result.IsSuccess)
            {
                logger.Info("Chain {Name} succeeded in {Elapsed} ms", context.ChainName, stopwatch.ElapsedMilliseconds);
            }
            else if (result.IsCanceled)
            {
                logger.Warning("Chain {Name} canceled after {Elapsed} ms", context.ChainName, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logger.Warning("Chain {Name} failed in {Elapsed} ms: {Error}",
                    context.ChainName, stopwatch.ElapsedMilliseconds, result.ErrorMessage ?? "Unknown Error");
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            logger.Warning("Chain {Name} canceled after {Elapsed} ms (OperationCanceledException)",
                context.ChainName, stopwatch.ElapsedMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.Error("Chain {Name} threw after {Elapsed} ms. Error: {Error}",
                context.ChainName, stopwatch.ElapsedMilliseconds, ex.Message);
            throw;
        }
    }
}
