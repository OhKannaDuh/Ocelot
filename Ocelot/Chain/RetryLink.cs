using System;
using System.Threading.Tasks;

namespace Ocelot.Chain;

public class RetryLink : IChainlink
{
    private readonly IChainlink task;
    private readonly int maxRetries;
    private readonly int delayBetweenRetriesMs;
    private readonly Func<Exception, bool>? retryCondition;

    public RetryLink(IChainlink task, int maxRetries, int delayBetweenRetriesMs = 0, Func<Exception, bool>? retryCondition = null)
    {
        this.task = task;
        this.maxRetries = maxRetries;
        this.delayBetweenRetriesMs = delayBetweenRetriesMs;
        this.retryCondition = retryCondition;
    }

    public async Task RunAsync(ChainContext context)
    {
        int attempts = 0;

        while (attempts < maxRetries)
        {
            try
            {
                Logger.Debug($"Running RetryLink attemp: ({attempts}/{maxRetries})");
                Logger.Debug($"Task type: {task.GetType().FullName}");
                await task.RunAsync(context);
                Logger.Debug($"Done");
                return;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception thrown inside retry link");
                if (retryCondition != null && !retryCondition(ex))
                    throw;

                attempts++;
                if (attempts >= maxRetries)
                    throw;

                if (delayBetweenRetriesMs > 0)
                    await Task.Delay(delayBetweenRetriesMs, context.token);
            }
        }
    }
}
