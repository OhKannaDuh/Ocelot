namespace Ocelot.Services.Translation;

public interface ITranslator
{
    event Action? LanguageChanged;

    event Action? TranslationsChanged;

    string Scope { get; }

    ITranslator WithScope(string scope);

    string T(string key);

    string T(string key, params (string key, object value)[] replacements);

    bool Has(string key);
}

public interface ITranslator<out TScope> : ITranslator
{
}
