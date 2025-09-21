namespace Ocelot.Chain;

public abstract class ChainRecipe<TArgs>(IChainFactory chains) : IChainRecipe<TArgs>
{
    public abstract string Name { get; }

    protected abstract IChain Compose(IChain chain, TArgs path);

    public IChain Build(TArgs args)
    {
        return Compose(chains.Create(Name), args);
    }
}

public abstract class ChainRecipe(IChainFactory chains) : IChainRecipe
{
    public abstract string Name { get; }

    protected abstract IChain Compose(IChain chain);

    public IChain Build()
    {
        return Compose(chains.Create(Name));
    }
}
