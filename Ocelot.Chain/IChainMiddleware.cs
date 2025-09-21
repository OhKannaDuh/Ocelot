namespace Ocelot.Chain;

public delegate Task<ChainResult> ChainMiddlewareDelegate();

public interface IChainMiddleware
{
    Task<ChainResult> InvokeAsync(IChainContext context, ChainMiddlewareDelegate next);
}
