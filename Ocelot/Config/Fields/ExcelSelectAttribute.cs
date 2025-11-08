using Lumina.Excel;
using Ocelot.Config.Renderers;
using Ocelot.Config.Renderers.Excel;

namespace Ocelot.Config.Fields;

public class ExcelSelectAttribute<TRow, TDisplay, TFilter>()
    : UIFieldAttribute(typeof(ExcelSelectRenderer<TRow, TDisplay, TFilter>))
    where TRow : struct, IExcelRow<TRow>
    where TDisplay : IExcelDisplay<TRow>
    where TFilter : IExcelFilter<TRow>;

public class ExcelSelectAttribute<TRow>
    : ExcelSelectAttribute<TRow, GenericDisplay<TRow>, NoOpFilter<TRow>>
    where TRow : struct, IExcelRow<TRow>;

public class ExcelSelectDisplayAttribute<TRow, TDisplay>
    : ExcelSelectAttribute<TRow, TDisplay, NoOpFilter<TRow>>
    where TRow : struct, IExcelRow<TRow>
    where TDisplay : IExcelDisplay<TRow>;

public class ExcelSelectFilterAttribute<TRow, TFilter>
    : ExcelSelectAttribute<TRow, GenericDisplay<TRow>, TFilter>
    where TRow : struct, IExcelRow<TRow>
    where TFilter : IExcelFilter<TRow>;
