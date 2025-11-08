namespace Ocelot.Services.Translation;

public interface ITranslationRepository
{
    event Action? LanguageChanged;

    event Action? TranslationsChanged;

    string CurrentLanguage { get; }

    IReadOnlyCollection<string> AvailableLanguages { get; }

    void LoadFromDirectory(string directory, string language);

    void SetLanguage(string language);

    bool TryGet(string key, out string value);

    string Get(string key, string? @default = null);

    bool Has(string key);

    void Reload();
}
