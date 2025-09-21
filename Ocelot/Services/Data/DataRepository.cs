using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ocelot.Services.Data;

public class DataRepository<TKey, TModel> : IDataRepository<TKey, TModel>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TModel> data = new();

    public IEnumerable<TKey> GetKeys()
    {
        return data.Keys;
    }

    public void Add(TKey key, TModel model)
    {
        data.Add(key, model);
    }

    public bool TryAdd(TKey key, TModel model)
    {
        return data.TryAdd(key, model);
    }

    public TModel Get(TKey key)
    {
        if (data.TryGetValue(key, out var result))
        {
            return result;
        }

        throw new KeyNotFoundException($"Key {key} not found");
    }

    public IEnumerable<TModel> GetAll()
    {
        return data.Values;
    }

    public IEnumerable<TModel> Where(Expression<Func<TModel, bool>> predicate)
    {
        return data.Values.Where(predicate.Compile());
    }

    public bool Remove(TKey key)
    {
        return data.Remove(key);
    }

    public bool ContainsKey(TKey key)
    {
        return data.ContainsKey(key);
    }
}
