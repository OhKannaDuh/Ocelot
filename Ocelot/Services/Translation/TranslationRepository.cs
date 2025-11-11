using System.Collections.Concurrent;
using System.Text.Json;
using Dalamud.Plugin;
using Ocelot.Services.Logger;

namespace Ocelot.Services.Translation;

public sealed class TranslationRepository(IDalamudPluginInterface plugin, ILogger<TranslationRepository> logger) : ITranslationRepository
{
    private readonly Lock gate = new();

    public event Action? LanguageChanged;

    public event Action? TranslationsChanged;

    private readonly ConcurrentDictionary<string, Dictionary<string, string>> byLang = new(StringComparer.Ordinal);

    private volatile Dictionary<string, string> active = new(StringComparer.Ordinal);

    private readonly List<(string Directory, string Language)> loadedDirectories = [];

    private string currentLanguage = "en";

    public string CurrentLanguage
    {
        get => currentLanguage;
    }

    public IReadOnlyCollection<string> AvailableLanguages
    {
        get => byLang.Keys.ToArray();
    }

    public void LoadFromDirectory(string directory, string language)
    {
        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new ArgumentNullException(nameof(directory));
        }

        if (string.IsNullOrWhiteSpace(language))
        {
            throw new ArgumentNullException(nameof(language));
        }

        loadedDirectories.Add((directory, language));

        directory = Path.Combine(plugin.AssemblyLocation.DirectoryName!, directory);

        var dir = new DirectoryInfo(directory);
        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(directory);
        }


        var chain = BuildFallbackChain(language);

        var merged = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var lang in chain)
        {
            var langFolder = Path.Combine(dir.FullName, lang);
            if (!Directory.Exists(langFolder))
            {
                continue;
            }

            var files = Directory.EnumerateFiles(langFolder, "*.json", SearchOption.AllDirectories)
                .OrderBy(p => p, StringComparer.Ordinal)
                .Select(p => (p, p.Replace(langFolder, "").Replace("/", ".").Replace("\\", ".").Substring(1)))
                .ToList();

            foreach (var file in files)
            {
                logger.Log(file.Item2);
            }
                
            foreach (var file in files)
            {
                logger.Debug("Loading file {f}", file);

                using var fs = File.OpenRead(file.p);
                using var doc = JsonDocument.Parse(fs, new JsonDocumentOptions { AllowTrailingCommas = true });

                var prefix = Path.GetFileNameWithoutExtension(file.Item2);
                if (prefix.StartsWith("ocelot."))
                {
                    prefix = prefix.Substring(7);
                }

                FlattenInto(doc.RootElement, merged, prefix);
            }

            lock (gate)
            {
                if (!byLang.TryGetValue(lang, out var snap))
                {
                    byLang[lang] = new Dictionary<string, string>(merged, StringComparer.Ordinal);
                }
                else
                {
                    byLang[lang] = new Dictionary<string, string>(merged, StringComparer.Ordinal);
                }
            }
        }

        lock (gate)
        {
            currentLanguage = language;
            active = new Dictionary<string, string>(merged, StringComparer.Ordinal);
            byLang[language] = active;
            TranslationsChanged?.Invoke();
        }
    }

    public void SetLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            throw new ArgumentNullException(nameof(language));
        }

        if (!byLang.TryGetValue(language, out var map))
        {
            throw new InvalidOperationException($"Language '{language}' not loaded. Call LoadFromDirectory first.");
        }

        lock (gate)
        {
            currentLanguage = language;
            active = new Dictionary<string, string>(map, StringComparer.Ordinal);
            LanguageChanged?.Invoke();
        }
    }

    public bool TryGet(string key, out string value)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return active.TryGetValue(key, out value!);
    }

    public string Get(string key, string? @default = null)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return active.TryGetValue(key, out var v) ? v : @default ?? key;
    }

    public bool Has(string key)
    {
        return active.ContainsKey(key);
    }

    public void Reload()
    {
        List<(string Directory, string Language)> snapshot;
        string previousLanguage;

        lock (gate)
        {
            snapshot = new List<(string Directory, string Language)>(loadedDirectories);
            previousLanguage = currentLanguage;

            loadedDirectories.Clear();
            byLang.Clear();
            active = new Dictionary<string, string>(StringComparer.Ordinal);
        }

        foreach (var (directory, language) in snapshot)
        {
            LoadFromDirectory(directory, language);
        }

        if (byLang.TryGetValue(previousLanguage, out _))
        {
            SetLanguage(previousLanguage);
        }

        TranslationsChanged?.Invoke();
    }

    private static IEnumerable<string> BuildFallbackChain(string language)
    {
        var parts = language.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            yield break;
        }

        for (var i = 1; i <= parts.Length; i++)
        {
            yield return string.Join('-', parts.Take(i));
        }
    }

    private static void FlattenInto(JsonElement element, IDictionary<string, string> output, string? prefix)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var prop in element.EnumerateObject())
                {
                    var key = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
                    FlattenInto(prop.Value, output, key);
                }

                break;

            case JsonValueKind.Array:
                var idx = 0;
                foreach (var item in element.EnumerateArray())
                {
                    var key = string.IsNullOrEmpty(prefix) ? $"{idx}" : $"{prefix}.{idx}";
                    FlattenInto(item, output, key);
                    idx++;
                }

                break;

            case JsonValueKind.String:
                output[prefix ?? ""] = element.GetString() ?? string.Empty;
                break;

            case JsonValueKind.Number:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                output[prefix ?? ""] = element.ToString();
                break;
        }
    }
}
