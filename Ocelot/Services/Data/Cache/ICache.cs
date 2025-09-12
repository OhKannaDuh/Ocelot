using System;

namespace Ocelot.Services.Data.Cache;

public interface ICache<in TKey, TModel>
{
    ICachePolicy CachePolicy { get; }

    bool TryGet(TKey key, out TModel value);

    TModel GetOrAdd(TKey key, Func<TModel> factory);

    void Set(TKey key, TModel value);

    void Remove(TKey key);
}
