using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ocelot.Chain;

public class ChainBuilder
{
    private readonly List<IChainlink> chain = new();

    private ChainBuilder() { }

    public static ChainBuilder Begin() => new();

    public ChainBuilder Then(Action action)
    {
        chain.Add(new ActionLink(action));
        return this;
    }

    public ChainBuilder Then(Func<Task> action)
    {
        chain.Add(new ActionLink(action));
        return this;
    }

    public ChainBuilder Wait(int milliseconds)
    {
        chain.Add(new DelayLink(milliseconds));
        return this;
    }

    public ChainBuilder Retry(Action action, int retries, int delayMs = 0)
    {
        chain.Add(new RetryLink(new ActionLink(action), retries, delayMs));
        return this;
    }

    public ChainBuilder Retry(Func<Task> action, int retries, int delayMs = 0)
    {
        chain.Add(new RetryLink(new ActionLink(action), retries, delayMs));
        return this;
    }

    public ChainBuilder AddLink(IChainlink link)
    {
        chain.Add(link);
        return this;
    }

    public Chain Build()
    {
        var chain = new Chain();
        chain.AddRange(this.chain);
        return chain;
    }
}
