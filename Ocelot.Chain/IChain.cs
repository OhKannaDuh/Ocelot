namespace Ocelot.Chain;

public interface IChain
{
    IChain Then(IStep step);

    IChain Then(IChain chain);

    IChain Then<T>() where T : class;

    IChain Then<TRecipe, TArgs>(TArgs args) where TRecipe : class, IChainRecipe<TArgs>;

    IChain UseMiddleware(IChainMiddleware middleware);

    IChain UseStepMiddleware(IStepMiddleware middleware);

    IChain UseMiddleware<TMiddleware>() where TMiddleware : class, IChainMiddleware;

    IChain UseStepMiddleware<TMiddleware>() where TMiddleware : class, IStepMiddleware;

    Task<ChainResult> ExecuteAsync(IChainContext context);

    Task<ChainResult> ExecuteAsync(CancellationToken cancellationToken = default);
}
