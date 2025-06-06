using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ocelot.Chain;

public class ChainBuilder
{
    private readonly List<IChainlink> chain = new();

    private ChainBuilder() { }

    public bool debug { get; private set; } = false;

    public static ChainBuilder Begin() => new();

    public ChainBuilder EnableDebug()
    {
        debug = true;
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
