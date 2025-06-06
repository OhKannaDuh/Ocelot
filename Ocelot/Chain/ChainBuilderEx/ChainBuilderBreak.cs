using System;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderBreak
{
    public static ChainBuilder BreakIf(this ChainBuilder builder, Func<bool> condition)
    {
        return builder
            .Debug($"Breaking if {condition}")
            .AddLink(new BreakIfLink(condition));
    }
}
