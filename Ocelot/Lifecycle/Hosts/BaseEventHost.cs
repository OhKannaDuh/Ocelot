using System;
using System.Collections.Generic;
using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public abstract class BaseEventHost(ILogger logger) : IEventHost
{
    public abstract void Start();

    public abstract void Stop();

    public abstract int Count { get; }

    protected void SafeEach<T>(IEnumerable<T> hooks, Action<T> call)
    {
        foreach (var h in hooks)
        {
            try
            {
                call(h);
            }
            catch (Exception ex)
            {
                logger.Error("Lifecycle hook {hook}, failed. Message: {message}", h?.GetType().Name ?? "Unknown", ex);
            }
        }
    }
}
