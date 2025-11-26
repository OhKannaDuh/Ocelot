using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.UI;

namespace Ocelot.UI.Services;

public static class IServiceCollectionExtensions
{
    public static void LoadUI(this IServiceCollection services)
    {
        services.AddSingleton<IBrandingService, DalamudBrandingService>();
        services.AddSingleton<IUIService, OcelotUIService>();
    }
}
