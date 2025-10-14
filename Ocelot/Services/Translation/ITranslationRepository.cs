namespace Ocelot.Services.Translation;

public interface ITranslationRepository
{
    string CurrentLanguage { get; }

    IReadOnlyCollection<string> AvailableLanguages { get; }

    void LoadFromDirectory(string directory, string language);

    void SetLanguage(string language);

    bool TryGet(string key, out string value);

    string Get(string key, string? @default = null);
}
