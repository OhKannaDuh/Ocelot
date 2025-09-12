namespace Ocelot.Services.Data.Cache;

public sealed class CacheEntry<TModel>(TModel value, CacheEntryMetadata metadata)
{
    public TModel Value { get; } = value;

    public CacheEntryMetadata Metadata { get; } = metadata;
}
