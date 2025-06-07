using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ocelot.Chain;

public class ChainBuilder
{
    private readonly Chain chain = new();

    internal readonly List<Action<Exception>> OnErrorHandlers = new();

    internal readonly List<Action> OnStopHandlers = new();

    internal readonly List<Action> OnFinallyHandlers = new();

    private ChainBuilder() { }

    public static ChainBuilder Begin() => new();

    public ChainBuilder AddLink(IChainlink link)
    {
        chain.Add(link);
        return this;
    }

    public Chain Build()
    {
        var chain = new Chain();
        chain.AddRange(this.chain);

        foreach (var h in OnErrorHandlers) chain.OnError(h);
        foreach (var h in OnStopHandlers) chain.OnStop(h);
        foreach (var h in OnFinallyHandlers) chain.OnFinally(h);

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
