using Lumina.Excel;

namespace Ocelot.Config.Handlers;

public abstract class ExcelSheetItemProvider<T> : IExcelSheetItemProvider<T>
    where T : struct, IExcelRow<T>
{
    public virtual bool Filter(T item) => true;

    public virtual string GetLabel(T item) => $"{typeof(T).Name} ({item.RowId})";
}
