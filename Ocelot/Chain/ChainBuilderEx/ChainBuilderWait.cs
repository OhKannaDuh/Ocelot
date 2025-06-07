using System;
using System.Threading.Tasks;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderWait
{
    public static ChainBuilder Wait(this ChainBuilder builder, int milliseconds) => builder.AddLink(new DelayLink(milliseconds));

    public static ChainBuilder WaitUntil(this ChainBuilder builder, Func<bool> condition, int timeout = 5000, int interval = 250)
        => builder.AddLink(new WaitUntilLink(condition, timeout, interval));

    public static ChainBuilder WaitWhile(this ChainBuilder builder, Func<bool> condition, int timeout = 5000, int interval = 250)
        => builder.AddLink(new WaitWhileLink(condition, timeout, interval));
}
