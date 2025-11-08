using Lumina.Excel;

namespace Ocelot.Config.Renderers.Excel;

public class GenericDisplay<TRow> : IExcelDisplay<TRow>
    where TRow : struct, IExcelRow<TRow>
{
    public string Display(TRow row)
    {
        return row.ToString() ?? row.RowId.ToString();
    }
}
