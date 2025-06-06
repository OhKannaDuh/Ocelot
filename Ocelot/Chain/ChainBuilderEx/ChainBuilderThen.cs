using System;
using System.Threading.Tasks;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderThen
{
    public static ChainBuilder Then(this ChainBuilder builder, Action action)
    {
        builder.AddLink(new ActionLink(action));
        return builder;
    }

    public static ChainBuilder Then(this ChainBuilder builder, Func<Task> action)
    {
        builder.AddLink(new ActionLink(action));
        return builder;
    }

    public static ChainBuilder ThenIf(this ChainBuilder builder, Action action, Func<bool> condition)
    {
        builder.AddLink(new ConditionalLink(condition, new ActionLink(action)));
        return builder;
    }

    public static ChainBuilder ThenOnFrameworkThread(this ChainBuilder builder, Action action)
    {
        builder.AddLink(new FrameworkThreadLink(action));
        return builder;
    }
}
