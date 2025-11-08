using Lumina.Excel;
using Ocelot.Config.Renderers;
using Ocelot.Config.Renderers.Excel;

namespace Ocelot.Config.Fields;

public class ExcelMultiSelectAttribute<TRow, TDisplay, TFilter>()
    : UIFieldAttribute(typeof(ExcelMultiSelectRenderer<TRow, TDisplay, TFilter>))
    where TRow : struct, IExcelRow<TRow>
    where TDisplay : IExcelDisplay<TRow>
    where TFilter : IExcelFilter<TRow>;

public class ExcelMultiSelectAttribute<TRow>
    : ExcelMultiSelectAttribute<TRow, GenericDisplay<TRow>, NoOpFilter<TRow>>
    where TRow : struct, IExcelRow<TRow>;

public class ExcelMultiSelectDisplayAttribute<TRow, TDisplay>
    : ExcelMultiSelectAttribute<TRow, TDisplay, NoOpFilter<TRow>>
    where TRow : struct, IExcelRow<TRow>
    where TDisplay : IExcelDisplay<TRow>;

public class ExcelMultiSelectFilterAttribute<TRow, TFilter>
    : ExcelMultiSelectAttribute<TRow, GenericDisplay<TRow>, TFilter>
    where TRow : struct, IExcelRow<TRow>
    where TFilter : IExcelFilter<TRow>;
