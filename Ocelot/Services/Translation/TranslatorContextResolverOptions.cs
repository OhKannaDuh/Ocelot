using System.Text.RegularExpressions;

namespace Ocelot.Services.Translation;

public class TranslatorContextResolverOptions(Type owner)
{
    public string Namespace { get; init; } = owner.FullName!.Split(".")[0];
    
    public Dictionary<Regex, Func<Type, string>> Replacements { get; init; } = new();
}
