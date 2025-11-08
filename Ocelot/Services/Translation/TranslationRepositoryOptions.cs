namespace Ocelot.Services.Translation;

public record TranslationRepositoryOptions
{
    public List<TranslationSource> Sources { get; init; } = [];
}
