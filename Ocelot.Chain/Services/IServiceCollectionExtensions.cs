using Microsoft.Extensions.DependencyInjection;
using Ocelot.Chain.Middleware.Chain;
using Ocelot.Chain.Middleware.Step;
using Ocelot.Chain.Recipes;
using Ocelot.Chain.Thread;

namespace Ocelot.Chain.Services;

public static class IServiceCollectionExtensions
{
    public static void LoadChain(this IServiceCollection services)
    {
        services.AddSingleton<IChainFactory, ChainFactory>();
        services.AddSingleton<IMainThread, DalamudMainThread>();

        services.AddTransient<LogChainMiddleware>();
        services.AddTransient<RetryChainMiddleware>();

        services.AddTransient<LogStepMiddleware>();
        services.AddTransient<RetryStepMiddleware>();
        services.AddTransient<RunOnMainThreadMiddleware>();

        services.AddSingleton<InteractChain>();
        services.AddSingleton<PathfindToChain>();
    }
}
