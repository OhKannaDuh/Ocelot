using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dalamud.Plugin.Services;
using Lumina.Excel;
using Ocelot.Services.Data.Cache;

namespace Ocelot.Services.Data;

public class ExcelDataRepository<TModel> : IDataRepository<TModel> where TModel : struct, IExcelRow<TModel>
{
    private readonly ExcelSheet<TModel> sheet;

    private ICache<uint, TModel> cache;

    private readonly Lazy<TModel[]> all;

    public ExcelDataRepository(IDataManager data, ICache<uint, TModel> cache)
    {
        sheet = data.GameData.GetExcelSheet<TModel>() ??
                throw new InvalidOperationException($"Unable to get excel sheet for {typeof(TModel).FullName ?? "Unknown"}");
        all = new Lazy<TModel[]>(() => sheet.ToArray());

        this.cache = cache;
    }

    public TModel Get(uint key)
    {
        return cache.GetOrAdd(key, () => sheet.GetRow(key));
    }

    public IEnumerable<TModel> GetAll()
    {
        return all.Value;
    }

    public IEnumerable<TModel> Where(Expression<Func<TModel, bool>> predicate)
    {
        return GetAll().Where(predicate.Compile());
    }
}
