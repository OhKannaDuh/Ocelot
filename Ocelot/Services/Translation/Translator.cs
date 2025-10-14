using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ocelot.Services.Translation;

public sealed class Translator(ITranslationRepository translations, string scope = "") : ITranslator
{
    private readonly ITranslationRepository translations = translations ?? throw new ArgumentNullException(nameof(translations));

    public string Scope { get; } = NormalizeScope(scope);

    public ITranslator WithScope(string scope)
    {
        if (string.IsNullOrWhiteSpace(scope))
        {
            return this;
        }

        var normalized = NormalizeScope(scope);
        if (Scope.Length == 0)
        {
            return new Translator(translations, normalized);
        }

        if (normalized.Length == 0)
        {
            return this;
        }

        return new Translator(translations, $"{Scope}.{normalized}");
    }

    public string T(string key)
    {
        var fullKey = BuildFullKey(Scope, key);
        return translations.Get(fullKey, key);
    }

    public string T(string key, params (string key, object value)[] replacements)
    {
        var fullKey = BuildFullKey(Scope, key);
        var template = translations.Get(fullKey, key);
        return ApplyReplacements(template, replacements);
    }


    private static string NormalizeScope(string scope)
    {
        if (string.IsNullOrWhiteSpace(scope))
        {
            return string.Empty;
        }

        return scope.Trim().Trim('.');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string BuildFullKey(string scope, string key)
    {
        if (key.Length > 0 && key[0] == '.')
        {
            var rel = key.Length > 1 ? key[1..] : string.Empty;
            if (string.IsNullOrEmpty(scope))
            {
                return rel;
            }

            if (string.IsNullOrEmpty(rel))
            {
                return scope;
            }

            return $"{scope}.{rel}";
        }

        return key;
    }

    private static string ApplyReplacements(string template, ReadOnlySpan<(string key, object value)> replacements)
    {
        if (string.IsNullOrEmpty(template) || replacements.Length == 0)
        {
            return template;
        }

        var map = new Dictionary<string, object>(replacements.Length, StringComparer.Ordinal);
        foreach (var (k, v) in replacements)
        {
            if (!string.IsNullOrEmpty(k))
            {
                map[k] = v;
            }
        }

        var sb = new StringBuilder(template.Length + 16);
        for (var i = 0; i < template.Length; i++)
        {
            var c = template[i];

            if (c != '{')
            {
                if (c == '}' && i + 1 < template.Length && template[i + 1] == '}')
                {
                    sb.Append('}');
                    i++;
                }
                else
                {
                    sb.Append(c);
                }

                continue;
            }

            if (i + 1 < template.Length && template[i + 1] == '{')
            {
                sb.Append('{');
                i++;
                continue;
            }

            var start = i + 1;
            var end = template.IndexOf('}', start);
            if (end == -1)
            {
                sb.Append(template.AsSpan(i));
                break;
            }

            var name = template.AsSpan(start, end - start).Trim();
            if (name.Length == 0)
            {
                sb.Append("{}");
            }
            else
            {
                var key = name.ToString();
                if (map.TryGetValue(key, out var value))
                {
                    sb.Append(Convert.ToString(value, CultureInfo.CurrentUICulture));
                }
                else
                {
                    sb.Append('{').Append(key).Append('}'); 
                }
            }

            i = end;
        }

        return sb.ToString();
    }
}
