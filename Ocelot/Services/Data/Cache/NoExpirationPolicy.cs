using System;

namespace Ocelot.Services.Data.Cache;

public class NoExpirationPolicy : ICachePolicy
{
    public bool IsExpired(CacheEntryMetadata metadata)
    {
        return false;
    }

    public TimeSpan? GetTimeToLive(object value)
    {
        return null;
    }
}
