namespace Ocelot.Services.Translation;

public interface ITranslatorContextResolver
{
    string ResolveScope(Type context);
}
