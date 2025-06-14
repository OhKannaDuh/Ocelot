using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using FFXIVClientStructs;
using Lumina.Data;

namespace Ocelot;

public static class I18N
{
    private static readonly Dictionary<string, Dictionary<string, string>> translations = new();

    private static readonly HashSet<string> reportedMissingKeys = new();

    private static string currentLanguage = "en";

    private static string fallbackLanguage = "en";

    private static string directory = "";

    public static void SetLanguage(string language)
    {
        currentLanguage = language;
    }

    public static void SetFallbackLanguage(string language)
    {
        fallbackLanguage = language;
    }

    public static void SetDirectory(string directory)
    {
        I18N.directory = directory;
    }

    public static void AddTranslations(string language, Dictionary<string, string> translations)
    {
        I18N.translations[language] = translations;
    }

    public static void LoadFromFile(string language, string path)
    {
        path = Path.Combine(directory, path);

        try
        {
            if (!File.Exists(path))
            {
                Logger.Warning($"[I18n] File not found: {path}");
                return;
            }

            var json = File.ReadAllText(path);
            LoadFromJson(language, json);
        }
        catch (Exception ex)
        {
            Logger.Error($"[I18n] Failed to load file for '{language}' from '{path}': {ex.Message}");
        }
    }

    public static void LoadFromJson(string language, string json)
    {
        try
        {
            var node = JsonNode.Parse(json);
            if (node is JsonObject root)
            {
                var flat = new Dictionary<string, string>();
                FlattenJson(root, flat);
                translations[language] = flat;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[I18n] Failed to load JSON for '{language}': {ex.Message}");
        }
    }

    private static void FlattenJson(JsonObject obj, Dictionary<string, string> result, string prefix = "")
    {
        foreach (var kvp in obj)
        {
            var key = string.IsNullOrEmpty(prefix) ? kvp.Key : $"{prefix}.{kvp.Key}";

            if (kvp.Value is JsonObject nestedObj)
            {
                FlattenJson(nestedObj, result, key);
            }
            else if (kvp.Value is JsonValue value)
            {
                result[key] = value.ToString();
            }
        }
    }

    public static string T(string key)
    {
        if (translations.TryGetValue(currentLanguage, out var currentDict) && currentDict.TryGetValue(key, out var value))
        {
            return value;
        }

        if (translations.TryGetValue(fallbackLanguage, out var fallbackDict) && fallbackDict.TryGetValue(key, out var fallbackValue))
        {
            Logger.Warning($"A translation for {key} was not found for the language {currentLanguage}, {fallbackLanguage} used instead.");

            var fallbackMessageKey = $"{currentLanguage}|{key}";
            if (reportedMissingKeys.Add(fallbackMessageKey))
            {
                Logger.Warning($"A translation for '{key}' was not found for the language '{currentLanguage}', fallback '{fallbackLanguage}' used instead.");
            }

            return fallbackValue;
        }

        var missingMessageKey = $"{currentLanguage}|{fallbackLanguage}|{key}";
        if (reportedMissingKeys.Add(missingMessageKey))
        {
            Logger.Warning($"A translation for '{key}' was not found for the language '{currentLanguage}' or fallback '{fallbackLanguage}'.");
        }

        return $"Unknown translation key: [[{key}]]";
    }
    public static string T(string key, Dictionary<string, string> replacements)
    {
        string template = T(key);

        foreach (var pair in replacements)
        {
            template = template.Replace($"{{{pair.Key}}}", pair.Value);
        }

        return template;
    }
}
