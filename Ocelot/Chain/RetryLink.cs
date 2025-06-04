using System;
using System.Threading.Tasks;

namespace Ocelot.Chain;

public class RetryLink : IChainlink
{
    private readonly IChainlink task;
    private readonly int maxRetries;
    private readonly int delayBetweenRetriesMs;

    public RetryLink(IChainlink task, int maxRetries, int delayBetweenRetriesMs = 0)
    {
        this.task = task;
        this.maxRetries = maxRetries;
        this.delayBetweenRetriesMs = delayBetweenRetriesMs;
    }

    public async Task RunAsync(ChainContext context)
    {
        int attempts = 0;

        while (attempts < maxRetries)
        {
            try
            {
                await task.RunAsync(context);
                return;
            }
            catch
            {
                attempts++;
                if (attempts >= maxRetries)
                {
                    throw;
                }

                if (delayBetweenRetriesMs > 0)
                {
                    await Task.Delay(delayBetweenRetriesMs, context.token);
                }
            }
        }
    }
}
