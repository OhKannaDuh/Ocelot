using Microsoft.Extensions.DependencyInjection;

namespace Ocelot.ECommons.Services;

public static class IServiceCollectionExtensions
{
    public static void LoadECommons(this IServiceCollection services)
    {
        services.AddSingleton<IECommonsInitProvider, ECommonsInitProvider>();
        services.AddSingleton<ECommons>();
    }
}
