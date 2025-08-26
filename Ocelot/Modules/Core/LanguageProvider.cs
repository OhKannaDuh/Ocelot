using System.Collections.Generic;
using Ocelot.Config.Handlers;

namespace Ocelot.Modules;

public class LanguageProvider : ListProvider<string>
{
    private readonly Dictionary<string, string> knownLanguages = new()
    {
        { "en", "English" },
        { "de", "Deutsch" },
        { "fr", "Français" },
        { "jp", "日本語" },
        { "zh", "中文" },
        { "uwu", "uwu" },
    };

    public override IEnumerable<string> GetItems()
    {
        return I18N.GetAllLanguageKeys();
    }

    public override bool Filter(string item)
    {
        return true;
    }

    public override string GetLabel(string item)
    {
        if (knownLanguages.TryGetValue(item, out var language))
        {
            return $"{item} ({language})";
        }

        return item;
    }
}
