using Lumina.Excel;

namespace Ocelot.Config.Renderers.Excel;

public interface IExcelDisplay<in TRow>
    where TRow : struct, IExcelRow<TRow>
{
    string Display(TRow row);
}
