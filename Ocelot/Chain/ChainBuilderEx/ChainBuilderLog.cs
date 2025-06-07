using System;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderLog
{
    public static ChainBuilder Log(this ChainBuilder builder, string message) => builder.AddLink(new LogLink(message));

    public static ChainBuilder Log(this ChainBuilder builder, Func<string> message) => builder.AddLink(new LogLink(message));

    public static ChainBuilder Debug(this ChainBuilder builder, string message) => builder.AddLink(new LogLink($"[ChainBuilderDebug] {message}"));

    public static ChainBuilder Debug(this ChainBuilder builder, Func<string> message) => builder.AddLink(new LogLink(() => $"[ChainBuilderDebug] {message()}"));
}
