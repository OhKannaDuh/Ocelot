using System;
using System.Collections.Generic;

namespace Ocelot.Services.Translation;

public interface ITranslationService
{
    event Action<string, string>? LanguageChanged;

    string CurrentLanguage { get; }

    string FallbackLanguage { get; }

    IEnumerable<string> Languages { get; }

    bool HasLanguage(string language);

    void SetLanguage(string language, string fallback = "en");

    void SetFallbackLanguage(string language);

    void SetDirectory(string directory);

    void AddTranslations(string language, IReadOnlyDictionary<string, string> newTranslations);

    void LoadFromFile(string language, string path);

    void LoadFromJson(string language, string json);

    void LoadAllFromDirectory(string language, string relativeDirectory);

    void LoadAllFromDirectory(string language);

    string T(string key);

    string? TOrNull(string key);

    string T(string key, IReadOnlyDictionary<string, string> replacements);
}
