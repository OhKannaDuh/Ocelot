using Lumina.Excel;

namespace Ocelot.Config.Renderers.Excel;

public class NoOpFilter<TRow> : IExcelFilter<TRow> where TRow : struct, IExcelRow<TRow>
{
    public bool Filter(TRow row)
    {
        return row.RowId > 0;
    }
}
