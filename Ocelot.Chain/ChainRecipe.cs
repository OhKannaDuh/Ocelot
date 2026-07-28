namespace Ocelot.Chain;

public abstract class ChainRecipe<TArgs>(IChainFactory chains) : IChainRecipe<TArgs>
{
    protected IChainFactory Chains { get; } = chains;

    public abstract string Name { get; }

    protected abstract IChain Compose(IChain chain, TArgs path);

    public IChain Build(TArgs args)
    {
        return Compose(Chains.Create(Name), args);
    }
}

public abstract class ChainRecipe(IChainFactory chains) : IChainRecipe
{
    protected IChainFactory Chains { get; } = chains;

    public abstract string Name { get; }

    protected abstract IChain Compose(IChain chain);

    public IChain Build()
    {
        return Compose(Chains.Create(Name));
    }
}
