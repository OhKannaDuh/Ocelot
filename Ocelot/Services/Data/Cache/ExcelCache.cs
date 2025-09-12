using Lumina.Excel;

namespace Ocelot.Services.Data.Cache;

public sealed class ExcelCache<TKey, TModel>() : GenericCache<TKey, TModel>(null)
    where TKey : notnull
    where TModel : struct, IExcelRow<TModel>;
