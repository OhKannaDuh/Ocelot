using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Ocelot.Lifecycle;

public static class HookProviderExtensions
{
    public static T[] GetOrderedHooks<T>(this IServiceProvider services)
        where T : IOrderedHook
    {
        return services.GetServices<T>().OrderByDescending(h => h.Order).ToArray();
    }


    public static T[] GetReverseOrderedHooks<T>(this IServiceProvider services)
        where T : IOrderedHook
    {
        return services.GetServices<T>().OrderBy(h => h.Order).ToArray();
    }
}
