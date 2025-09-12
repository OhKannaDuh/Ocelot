using System;

namespace Ocelot.Services.Data.Cache;

public class CacheEntryMetadata
{
    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? ExpiresAt { get; init; }

    public int HitCount;
}
