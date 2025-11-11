namespace Ocelot.Services.Translation.Extensions;

public static class ITranslatorGenericExtensions
{
    public static string Start(this ITranslator translator)
    {
        return translator.T("generic.start");
    }

    public static string Stop(this ITranslator translator)
    {
        return translator.T("generic.stop");
    }

    public static string Enabled(this ITranslator translator)
    {
        return translator.T("generic.enabled");
    }

    public static string Disabled(this ITranslator translator)
    {
        return translator.T("generic.disabled");
    }

    public static string Toggle(this ITranslator translator)
    {
        return translator.T("generic.toggle");
    }

    public static string ToggleConfigWindow(this ITranslator translator)
    {
        return translator.T("generic.toggle_config_window");
    }

    public static string EmergencyStop(this ITranslator translator)
    {
        return translator.T("generic.emergency_stop");
    }

    public static string Yes(this ITranslator translator)
    {
        return translator.T("generic.yes");
    }

    public static string No(this ITranslator translator)
    {
        return translator.T("generic.no");
    }

    public static string State(this ITranslator translator)
    {
        return translator.T("generic.state");
    }
}
