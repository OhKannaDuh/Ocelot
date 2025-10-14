using System.Collections.Concurrent;

namespace Ocelot.Services.Data.Cache;

public class GenericCache<TKey, TModel>(ICachePolicy? policy) : ICache<TKey, TModel> where TKey : notnull
{
    public ICachePolicy CachePolicy { get; } = policy ?? new NoExpirationPolicy();

    private readonly ConcurrentDictionary<TKey, CacheEntry<TModel>> map = new();

    public bool TryGet(TKey key, out TModel value)
    {
        if (map.TryGetValue(key, out var e))
        {
            if (!CachePolicy.IsExpired(e.Metadata))
            {
                Interlocked.Increment(ref e.Metadata.HitCount);

                value = e.Value;
                return true;
            }

            map.TryRemove(key, out _);
        }

        value = default!;
        return false;
    }

    public TModel GetOrAdd(TKey key, Func<TModel> factory)
    {
        if (TryGet(key, out var existing))
        {
            return existing;
        }

        var created = factory();
        var now = DateTimeOffset.UtcNow;
        var ttl = CachePolicy.GetTimeToLive(created);
        var meta = new CacheEntryMetadata
        {
            CreatedAt = now,
            ExpiresAt = now + ttl,
            HitCount = 0,
        };

        var newEntry = new CacheEntry<TModel>(created, meta);

        while (true)
        {
            if (map.TryGetValue(key, out var cur))
            {
                if (!CachePolicy.IsExpired(cur.Metadata))
                {
                    return cur.Value;
                }

                if (map.TryUpdate(key, newEntry, cur))
                {
                    return created;
                }

                continue;
            }

            if (map.TryAdd(key, newEntry))
            {
                return created;
            }
        }
    }

    public void Set(TKey key, TModel value)
    {
        var now = DateTimeOffset.UtcNow;
        var ttl = CachePolicy.GetTimeToLive(value);
        var meta = new CacheEntryMetadata
        {
            CreatedAt = now,
            ExpiresAt = now + ttl,
            HitCount = 0,
        };

        map[key] = new CacheEntry<TModel>(value, meta);
    }

    public void Remove(TKey key)
    {
        map.TryRemove(key, out _);
    }
}
