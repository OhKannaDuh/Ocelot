using Ocelot.Services.Logger;

namespace Ocelot.Services.Translation;

public class ContextualTranslator<TScope> : ITranslator<TScope>
{
    private ITranslator inner;

    public ContextualTranslator(ITranslator inner, ITranslatorContextResolver contextResolver, ILogger<ContextualTranslator<TScope>> logger)
    {
        this.inner = inner.WithScope(contextResolver.ResolveScope(typeof(TScope)));
        logger.Verbose("Resolved translation scope {s} for {t}", Scope, typeof(TScope).FullName ?? typeof(TScope).Name);
    }

    public event Action? LanguageChanged
    {
        add => inner.LanguageChanged += value;
        remove => inner.LanguageChanged -= value;
    }

    public event Action? TranslationsChanged
    {
        add => inner.TranslationsChanged += value;
        remove => inner.TranslationsChanged -= value;
    }

    public string Scope
    {
        get => inner.Scope;
    }

    // Returns a new translator; changing "inner" here rescoped the shared singleton for every user of it.
    public ITranslator WithScope(string scope)
    {
        return inner.WithScope(scope);
    }

    public string T(string key)
    {
        return inner.T(key);
    }

    public string T(string key, params (string key, object value)[] replacements)
    {
        return inner.T(key, replacements);
    }

    public bool Has(string key)
    {
        return inner.Has(key);
    }
}
