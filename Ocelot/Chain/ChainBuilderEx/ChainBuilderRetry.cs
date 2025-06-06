using System;
using System.Threading.Tasks;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderRetry
{
    public static ChainBuilder Retry(this ChainBuilder builder, Action action, int retries, int delayMs = 0)
    {
        return builder
            .Debug($"Retrying action: {action.Method.DeclaringType?.FullName}.{action.Method.Name}")
            .AddLink(new RetryLink(new ActionLink(action), retries, delayMs));
    }

    public static ChainBuilder Retry(this ChainBuilder builder, Func<Task> action, int retries, int delayMs = 0)
    {
        return builder
            .Debug($"Retrying action: {action.Method.DeclaringType?.FullName}.{action.Method.Name}")
            .AddLink(new RetryLink(new ActionLink(action), retries, delayMs));
    }
}
