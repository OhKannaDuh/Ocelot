using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumina.Excel;

namespace Ocelot.Services.Data;

public interface IDataRepository<TKey, TModel>
    where TKey : notnull
{
    IEnumerable<TKey> GetKeys();
    
    void Add(TKey key, TModel model);
    
    bool TryAdd(TKey key, TModel model);
    
    TModel Get(TKey key);

    IEnumerable<TModel> GetAll();

    IEnumerable<TModel> Where(Expression<Func<TModel, bool>> predicate);
    
    bool Remove(TKey key);
    
    bool ContainsKey(TKey key);
}

public interface IDataRepository<TModel> : IDataRepository<uint, TModel> where TModel : struct, IExcelRow<TModel>;
