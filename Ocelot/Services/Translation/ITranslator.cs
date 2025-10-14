namespace Ocelot.Services.Translation;

public interface ITranslator
{
    string Scope { get; }

    ITranslator WithScope(string scope);

    string T(string key);

    string T(string key, params (string key, object value)[] replacements);
}
