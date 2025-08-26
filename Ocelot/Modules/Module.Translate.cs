using System.Linq;

namespace Ocelot.Modules;

public abstract partial class Module<P, C>
    where P : OcelotPlugin
    where C : OcelotConfig
{
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
