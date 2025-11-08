namespace Ocelot.Chain;

public class ChainFactory(IServiceProvider services) : IChainFactory
{
    public IChain Create(string name)
    {
        return new Chain(name, services);
    }
}
