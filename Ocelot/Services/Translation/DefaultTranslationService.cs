using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading;
using Ocelot.Services.Logger;

namespace Ocelot.Services.Translation;

[OcelotService(typeof(ITranslationService), ServiceLifetime.Singleton)]
public sealed class DefaultTranslationService : ITranslationService
{
    private static ILoggerService Logger {
        get => OcelotServices.GetCached<ILoggerService>();
    }

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> translations = new();

    private readonly ConcurrentDictionary<string, byte> reportedMissingKeys = new();

    private readonly ReaderWriterLockSlim gate = new(LockRecursionPolicy.NoRecursion);

    private string currentLanguage = "en";

    private string fallbackLanguage = "en";

    private string baseDirectory = "";

    public event Action<string, string>? LanguageChanged;

    public string CurrentLanguage {
        get {
            gate.EnterReadLock();
            try
            {
                return currentLanguage;
            } finally
            {
                gate.ExitReadLock();
            }
        }
    }

    public string FallbackLanguage {
        get {
            gate.EnterReadLock();
            try
            {
                return fallbackLanguage;
            } finally
            {
                gate.ExitReadLock();
            }
        }
    }

    public IEnumerable<string> Languages {
        get => translations.Keys;
    }

    public bool HasLanguage(string language)
    {
        return translations.ContainsKey(language);
    }

    public void SetLanguage(string language, string fallback = "en")
    {
        string prev;
        gate.EnterWriteLock();
        try
        {
            if (!HasLanguage(language))
            {
                language = fallback;
            }

            prev = currentLanguage;
            currentLanguage = language;
        } finally
        {
            gate.ExitWriteLock();
        }

        LanguageChanged?.Invoke(prev, language);
    }

    public void SetFallbackLanguage(string language)
    {
        gate.EnterWriteLock();
        try
        {
            fallbackLanguage = language;
        } finally
        {
            gate.ExitWriteLock();
        }
    }

    public void SetDirectory(string directory)
    {
        gate.EnterWriteLock();

        try
        {
            baseDirectory = directory;
        } finally
        {
            gate.ExitWriteLock();
        }
    }

    public void AddTranslations(string language, IReadOnlyDictionary<string, string> newTranslations)
    {
        var dict = translations.GetOrAdd(language, _ => new ConcurrentDictionary<string, string>(StringComparer.Ordinal));
        foreach (var kv in newTranslations)
        {
            dict[kv.Key] = kv.Value;
        }
    }

    public void LoadFromFile(string language, string path)
    {
        string full;
        gate.EnterReadLock();
        try { full = Path.Combine(baseDirectory, path); } finally { gate.ExitReadLock(); }

        try
        {
            if (!File.Exists(full))
            {
                Logger.Warn($"[I18n] File not found: {full}");
                return;
            }

            var json = File.ReadAllText(full);
            LoadFromJson(language, json);
        }
        catch (Exception ex)
        {
            Logger.Error($"[I18n] Failed to load file for '{language}' from '{full}': {ex.Message}");
        }
    }

    public void LoadFromJson(string language, string json)
    {
        try
        {
            var node = JsonNode.Parse(json);
            if (node is JsonObject root)
            {
                var flat = new Dictionary<string, string>(StringComparer.Ordinal);
                FlattenJson(root, flat);

                var dict = translations.GetOrAdd(language, _ => new ConcurrentDictionary<string, string>(StringComparer.Ordinal));
                foreach (var kv in flat)
                {
                    dict[kv.Key] = kv.Value;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Warn($"[I18n] Failed to load JSON for '{language}': {ex.Message}");
        }
    }

    public void LoadAllFromDirectory(string language, string relativeDirectory)
    {
        string full;
        gate.EnterReadLock();
        try { full = Path.Combine(baseDirectory, relativeDirectory); } finally { gate.ExitReadLock(); }

        if (!Directory.Exists(full))
        {
            Logger.Warn($"[I18n] Directory not found: {full}");
            return;
        }

        var files = Directory.GetFiles(full, "*.json", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            try
            {
                var json = File.ReadAllText(file);
                LoadFromJson(language, json);
            }
            catch (Exception ex)
            {
                Logger.Error($"[I18n] Failed to load file '{file}' for '{language}': {ex.Message}");
            }
        }
    }

    public void LoadAllFromDirectory(string language)
    {
        LoadAllFromDirectory(language, language);
    }

    public string T(string key)
    {
        var value = TOrNull(key);
        if (value != null) return value;

        string cur,
               fb;
        gate.EnterReadLock();
        try
        {
            cur = currentLanguage;
            fb = fallbackLanguage;
        } finally
        {
            gate.ExitReadLock();
        }

        var missingKey = $"{cur}|{fb}|{key}";
        if (reportedMissingKeys.TryAdd(missingKey, 0))
        {
            Logger.Warn($"A translation for '{key}' was not found for the language '{cur}' or fallback '{fb}'.");
        }

        return $"Unknown translation key: [[{key}]]";
    }

    public string? TOrNull(string key)
    {
        string cur,
               fb;
        gate.EnterReadLock();
        try
        {
            cur = currentLanguage;
            fb = fallbackLanguage;
        } finally
        {
            gate.ExitReadLock();
        }

        if (translations.TryGetValue(cur, out var currentDict) && currentDict.TryGetValue(key, out var value))
        {
            return value;
        }

        if (translations.TryGetValue(fb, out var fallbackDict) && fallbackDict.TryGetValue(key, out var fallbackValue))
        {
            var fallbackMessageKey = $"{cur}|{key}";
            if (reportedMissingKeys.TryAdd(fallbackMessageKey, 0))
            {
                Logger.Warn($"A translation for '{key}' was not found for the language '{cur}', fallback '{fb}' used instead.");
            }

            return fallbackValue;
        }

        return null;
    }

    public string T(string key, IReadOnlyDictionary<string, string> replacements)
    {
        var template = T(key);
        if (replacements.Count == 0)
        {
            return template;
        }

        foreach (var pair in replacements)
        {
            template = template.Replace($"{{{pair.Key}}}", pair.Value, StringComparison.Ordinal);
        }

        return template;
    }

    private static void FlattenJson(JsonObject obj, IDictionary<string, string> result, string prefix = "")
    {
        foreach (var kvp in obj)
        {
            var key = string.IsNullOrEmpty(prefix) ? kvp.Key : $"{prefix}.{kvp.Key}";
            switch (kvp.Value)
            {
                case JsonObject nested:
                    FlattenJson(nested, result, key);
                    break;
                case JsonValue v:
                    result[key] = v.ToString();
                    break;
            }
        }
    }
}
