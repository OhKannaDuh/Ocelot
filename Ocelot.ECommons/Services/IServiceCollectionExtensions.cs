﻿using Microsoft.Extensions.DependencyInjection;
using Ocelot.ECommons.Services.Logger;
using Ocelot.Services.Logger;

namespace Ocelot.ECommons.Services;

public static class IServiceCollectionExtensions
{
    public static void LoadECommons(this IServiceCollection services)
    {
        services.AddSingleton<ECommons>();
        services.AddSingleton<ILoggerService, ECommonsLoggerService>();
    }
}
