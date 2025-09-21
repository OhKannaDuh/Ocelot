using Ocelot.Services.Logger;

namespace Ocelot.Chain.Middleware.Chain;

public sealed class RetryChainMiddleware(ILogger logger) : IChainMiddleware
{
    public int DelayMs { get; init; } = 0;

    public int MaxAttempts { get; init; } = 3;

    public Func<Exception, bool>? ShouldRetryOnException { get; init; }

    public Func<ChainResult, bool>? ShouldRetryOnResult { get; init; }

    public async Task<ChainResult> InvokeAsync(IChainContext context, ChainMiddlewareDelegate next)
    {
        var attempt = 0;

        while (true)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            attempt++;

            context.SetData("ChainAttempt", attempt);

            try
            {
                logger.Info("Chain attempt {Attempt} starting (RunId={RunId})", attempt, context.RunId);

                var result = await next();

                if (result.IsSuccess)
                {
                    logger.Info("Chain attempt {Attempt} succeeded (RunId={RunId})", attempt, context.RunId);
                    return result;
                }

                if (result.IsCanceled)
                {
                    logger.Warning("Chain attempt {Attempt} canceled (RunId={RunId})", attempt, context.RunId);
                    return result;
                }

                var shouldRetry = ShouldRetryOnResult?.Invoke(result) ?? true;
                if (!shouldRetry || attempt >= MaxAttempts)
                {
                    logger.Warning("Chain attempt {Attempt} failed; not retrying (RunId={RunId}). Error={Error}",
                        attempt, context.RunId, result.ErrorMessage ?? "Unknown Error");

                    return result;
                }
            }
            catch (OperationCanceledException)
            {
                logger.Warning("Chain attempt {Attempt} canceled by token (RunId={RunId})", attempt, context.RunId);
                throw;
            }
            catch (Exception ex)
            {
                var shouldRetry = ShouldRetryOnException?.Invoke(ex) ?? true;
                if (!shouldRetry || attempt >= MaxAttempts)
                {
                    logger.Error("Chain attempt {Attempt} threw; not retrying (RunId={RunId}). Error={Error}",
                        attempt, context.RunId, ex.Message);
                    throw;
                }
            }

            if (DelayMs > 0)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(DelayMs), context.CancellationToken);
            }
        }
    }
}
