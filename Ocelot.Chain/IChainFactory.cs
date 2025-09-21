namespace Ocelot.Chain;

public interface IChainFactory
{
    IChain Create(string name);
}
