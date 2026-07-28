using System.Globalization;
using System.Text.RegularExpressions;

namespace Ocelot.Extensions;

public static class StringExtensions
{
    private static readonly HashSet<string> TranslationKeySuffixes = new(StringComparer.Ordinal)
    {
        "label",
        "tooltip",
        "fields",
        "title",
    };

    public static string HumanizeTranslationKey(this string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return key;
        }

        var parts = key.Split('.');
        for (var i = parts.Length - 1; i >= 0; i--)
        {
            if (!TranslationKeySuffixes.Contains(parts[i]))
            {
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parts[i].Replace('_', ' '));
            }
        }

        return key;
    }

    public static string ToSnakeCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        var result = Regex.Replace(str, @"([a-z0-9])([A-Z])", "$1_$2");
        result = Regex.Replace(result, @"([A-Z]+)([A-Z][a-z])", "$1_$2");
        result = result.Replace(" ", "_").Replace("-", "_");

        return result.ToLowerInvariant();
    }

    public static string ToKebabCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        var result = Regex.Replace(str, @"([a-z0-9])([A-Z])", "$1-$2");
        result = Regex.Replace(result, @"([A-Z]+)([A-Z][a-z])", "$1-$2");
        result = result.Replace(" ", "-").Replace("_", "-");

        return result.ToLowerInvariant();
    }

    public static string WithPrefix(this string str, string prefix)
    {
        if (string.IsNullOrEmpty(prefix))
        {
            return str;
        }

        if (string.IsNullOrEmpty(str))
        {
            return prefix;
        }

        return str.StartsWith(prefix) ? str : prefix + str;
    }

    public static string WithSuffix(this string str, string suffix)
    {
        if (string.IsNullOrEmpty(suffix))
        {
            return str;
        }

        if (string.IsNullOrEmpty(str))
        {
            return suffix;
        }

        return str.EndsWith(suffix) ? str : str + suffix;
    }
}
