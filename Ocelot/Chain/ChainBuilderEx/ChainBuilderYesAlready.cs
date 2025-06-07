using Ocelot.IPC;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderYesAlready
{
    public static ChainBuilder DisableYesAlready(this ChainBuilder builder, YesAlready? yes)
    {
        if (yes == null)
        {
            return builder.Debug("Yes Already not installed");
        }

        if (!yes.IsPluginEnabled())
        {
            return builder.Debug("Yes Already not enabled");
        }

        return builder
                .Debug("Disabling Yes Already until chain finished")
                .OnFinally(() => yes.SetPluginEnabled(true))
                .Then(() => yes.SetPluginEnabled(false));
    }
}
