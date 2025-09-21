namespace Ocelot.Chain;

public interface IChainRecipe<in TArgs>
{
    string Name { get; }

    IChain Build(TArgs args);
}

public interface IChainRecipe
{
    string Name { get; }

    IChain Build();
}
