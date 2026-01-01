namespace Ocelot.Chain;

public interface IChainManager
{
    Task<ChainResult> ExecuteAsync(Func<IChainFactory, IChain> factory);

    Task<ChainResult> Manage(IChain chain);

    void CancelAll();
}
