using System;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderLog
{
    public static ChainBuilder Log(this ChainBuilder builder, string message)
    {
        builder.AddLink(new LogLink(message));
        return builder;
    }

    public static ChainBuilder Log(this ChainBuilder builder, Func<string> message)
    {
        builder.AddLink(new LogLink(message));
        return builder;
    }

    public static ChainBuilder Debug(this ChainBuilder builder, string message)
    {
        if (builder.debug)
        {
            builder.AddLink(new LogLink($"[ChainBuilderDebug] {message}"));
        }

        return builder;
    }

    public static ChainBuilder Debug(this ChainBuilder builder, Func<string> message)
    {
        if (builder.debug)
        {
            builder.AddLink(new LogLink(() => $"[ChainBuildDebug] {message()}"));
        }

        return builder;
    }
}
