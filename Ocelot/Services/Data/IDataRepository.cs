using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumina.Excel;

namespace Ocelot.Services.Data;

public interface IDataRepository<in TKey, TModel>
{
    TModel Get(TKey key);

    IEnumerable<TModel> GetAll();

    IEnumerable<TModel> Where(Expression<Func<TModel, bool>> predicate);
}

public interface IDataRepository<TModel> : IDataRepository<uint, TModel> where TModel : struct, IExcelRow<TModel>;
