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

    public ChainBuilder ThenIf(Action action, Func<bool> condition)
    {
        chain.Add(new ConditionalLink(condition, new ActionLink(action)));
        return this;
    }

    public ChainBuilder ThenIf(Func<Task> action, Func<bool> condition)
    {
        chain.Add(new ConditionalLink(condition, new ActionLink(action)));
        return this;
    }

    public ChainBuilder ThenOnFrameworkThread(Action action)
    {
        chain.Add(new FrameworkThreadLink(action));
        return this;
    }

    public ChainBuilder BreakIf(Func<bool> condition, string? reason = null)
    {
        chain.Add(new BreakIfLink(condition));
        return this;
    }

    public ChainBuilder Wait(int milliseconds)
    {
        chain.Add(new DelayLink(milliseconds));
        return this;
    }

    public ChainBuilder WaitUntil(Func<bool> condition, int timeout = 5000, int interval = 250)
    {
        chain.Add(new WaitUntilLink(condition, timeout, interval));
        return this;
    }

    public ChainBuilder WaitWhile(Func<bool> condition, int timeout = 5000, int interval = 250)
    {
        chain.Add(new WaitWhileLink(condition, timeout, interval));
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

    public ChainBuilder Log(string message)
    {
        chain.Add(new LogLink(message));
        return this;
    }

    public ChainBuilder Log(Func<string> message)
    {
        chain.Add(new LogLink(message));
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

    public ChainBuilder Merge(ChainBuilder other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        chain.AddRange(other.chain);
        return this;
    }

    public ChainBuilder Merge(Chain other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        chain.AddRange(other);
        return this;
    }

    public async Task Run() => ChainRunner.Run(Build());
}
