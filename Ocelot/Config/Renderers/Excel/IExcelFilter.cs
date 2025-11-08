using Lumina.Excel;

namespace Ocelot.Config.Renderers.Excel;

public interface IExcelFilter<in TRow>
    where TRow : struct, IExcelRow<TRow>
{
    bool Filter(TRow row);
}
