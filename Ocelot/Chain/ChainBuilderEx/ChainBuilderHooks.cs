using System;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderHooks
{
    public static ChainBuilder OnError(this ChainBuilder builder, Action<Exception> handler)
    {
        builder.OnErrorHandlers.Add(handler);
        return builder;
    }

    public static ChainBuilder OnStop(this ChainBuilder builder, Action handler)
    {
        builder.OnStopHandlers.Add(handler);
        return builder;
    }

    public static ChainBuilder OnFinally(this ChainBuilder builder, Action handler)
    {
        builder.OnFinallyHandlers.Add(handler);
        return builder;
    }
}
