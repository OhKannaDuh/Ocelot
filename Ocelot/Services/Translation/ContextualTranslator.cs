using Ocelot.Services.Logger;

namespace Ocelot.Services.Translation;

public class ContextualTranslator<TScope> : ITranslator<TScope>
{
    private ITranslator inner;

    public ContextualTranslator(ITranslator inner, ITranslatorContextResolver contextResolver, ILogger<ContextualTranslator<TScope>> logger)
    {
        this.inner = inner.WithScope(contextResolver.ResolveScope(typeof(TScope)));
        logger.Debug("Resolved translation scope {s} for {t}", Scope, typeof(TScope).FullName ?? typeof(TScope).Name);
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

    public ITranslator WithScope(string scope)
    {
        inner = inner.WithScope(scope);
        return this;
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
