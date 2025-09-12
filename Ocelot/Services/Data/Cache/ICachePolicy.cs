using System;

namespace Ocelot.Services.Data.Cache;

public interface ICachePolicy
{
    bool IsExpired(CacheEntryMetadata metadata);

    TimeSpan? GetTimeToLive(object value);
}
