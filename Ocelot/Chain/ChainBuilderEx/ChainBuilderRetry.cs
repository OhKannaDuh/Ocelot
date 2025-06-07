using System;
using System.Threading.Tasks;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderRetry
{
    public static ChainBuilder Retry(this ChainBuilder builder, Action action, int retries, int delayMs = 0, Func<Exception, bool>? retryCondition = null)
        => builder
            .Debug($"Retrying action: {action.Method.DeclaringType?.FullName}.{action.Method.Name}")
            .AddLink(new RetryLink(new ActionLink(action), retries, delayMs, retryCondition));


    public static ChainBuilder Retry(this ChainBuilder builder, Func<Task> action, int retries, int delayMs = 0, Func<Exception, bool>? retryCondition = null)
        => builder
            .Debug($"Retrying action: {action.Method.DeclaringType?.FullName}.{action.Method.Name}")
            .AddLink(new RetryLink(new ActionLink(action), retries, delayMs, retryCondition));


    public static ChainBuilder Retry(this ChainBuilder builder, IChainlink chain, int retries, int delayMs = 0, Func<Exception, bool>? retryCondition = null)
        => builder
            .AddLink(new RetryLink(chain, retries, delayMs, retryCondition));
}
