using Lumina.Excel;

namespace Ocelot.Config.Handlers;

public abstract class ExcelSheetItemProvider<T> : IExcelSheetItemProvider<T>
    where T : struct, IExcelRow<T>
{
    public bool Filter(T item) => true;

    public string GetLabel(T item) => $"{typeof(T).Name} ({item.RowId})";
}
