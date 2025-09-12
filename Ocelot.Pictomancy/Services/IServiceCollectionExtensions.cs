using Microsoft.Extensions.DependencyInjection;
using Ocelot.Pictomancy.Services.OverlayRenderer;
using Ocelot.Services.OverlayRenderer;

namespace Ocelot.Pictomancy.Services;

public static class IServiceCollectionExtensions
{
    public static void LoadPictomancy(this IServiceCollection services)
    {
        services.AddSingleton<Pictomancy>();
        services.AddSingleton<IPictomancyProvider, PictomancyProvider>();
        services.AddSingleton<IOverlayRendererService, OverlayRendererService>();
    }
}
