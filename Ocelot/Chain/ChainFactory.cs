using System;

namespace Ocelot.Chain;

public abstract class ChainFactory
{
    protected abstract Chain Create(Chain chain);

    public Func<Chain> Factory() => () => Create(Chain.Create(GetType().Name));
}