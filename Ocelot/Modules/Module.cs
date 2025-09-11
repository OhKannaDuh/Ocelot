using System.Linq;
using Ocelot.Services;
using Ocelot.Services.Logger;
using Ocelot.Services.Translation;

namespace Ocelot.Modules;

public abstract partial class Module<P, C>(P plugin, C pluginConfig) : IModule
    where P : OcelotPlugin
    where C : OcelotConfig
{
    public readonly P Plugin = plugin;

    public readonly C PluginConfig = pluginConfig;

    public virtual ModuleConfig? Config {
        get => null;
    }

    private static ILoggerService Logger {
        get => OcelotServices.GetCached<ILoggerService>();
    }

    private static ITranslationService I18N {
        get => OcelotServices.GetCached<ITranslationService>();
    }

    public void Debug(string log)
    {
        Logger.Debug($"[{GetType().Name}] {log}");
    }

    public void Error(string log)
    {
        Logger.Error($"[{GetType().Name}] {log}");
    }

    public void Fatal(string log)
    {
        Logger.Fatal($"[{GetType().Name}] {log}");
    }

    public void Info(string log)
    {
        Logger.Info($"[{GetType().Name}] {log}");
    }

    public void Verbose(string log)
    {
        Logger.Verbose($"[{GetType().Name}] {log}");
    }

    public void Warning(string log)
    {
        Logger.Warn($"[{GetType().Name}] {log}");
    }

    public string Translate(string key)
    {
        string ToSnakeCase(string input)
        {
            return string.Concat(input.Select((x, i) =>
                i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
        }

        var module = ToSnakeCase(GetType().Name).Replace("_module", "");
        var moduleKey = $"modules.{module}." + key;

        var moduleTranslation = I18N.TOrNull(moduleKey);
        if (moduleTranslation != null)
        {
            return moduleTranslation;
        }

        return I18N.TOrNull(key) ?? I18N.T(moduleKey);
    }

    public string Trans(string key)
    {
        return Translate(key);
    }

    public string T(string key)
    {
        return Translate(key);
    }
}
