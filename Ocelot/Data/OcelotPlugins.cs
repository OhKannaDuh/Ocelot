namespace Ocelot.Data;

public enum OcelotPlugins
{
    OcelotMonitor,
    BOCCHI,
    TwistOfFayte,
}

public static class OcelotPluginsEx
{
    public static string GetInternalName(this OcelotPlugins plugin)
    {
        return plugin.ToString();
    }
}
