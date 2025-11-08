using Lumina.Excel;
using Ocelot.Config.Renderers;
using Ocelot.Config.Renderers.Excel;

namespace Ocelot.Config.Fields;

public class ExcelOrderedMultiSelectAttribute<TRow, TDisplay, TFilter>()
    : UIFieldAttribute(typeof(ExcelOrderedMultiSelectRenderer<TRow, TDisplay, TFilter>))
    where TRow : struct, IExcelRow<TRow>
    where TDisplay : IExcelDisplay<TRow>
    where TFilter : IExcelFilter<TRow>;

public class ExcelOrderedMultiSelectAttribute<TRow>
    : ExcelOrderedMultiSelectAttribute<TRow, GenericDisplay<TRow>, NoOpFilter<TRow>>
    where TRow : struct, IExcelRow<TRow>;

public class ExcelOrderedMultiSelectDisplayAttribute<TRow, TDisplay>
    : ExcelOrderedMultiSelectAttribute<TRow, TDisplay, NoOpFilter<TRow>>
    where TRow : struct, IExcelRow<TRow>
    where TDisplay : IExcelDisplay<TRow>;

public class ExcelOrderedMultiSelectFilterAttribute<TRow, TFilter>
    : ExcelOrderedMultiSelectAttribute<TRow, GenericDisplay<TRow>, TFilter>
    where TRow : struct, IExcelRow<TRow>
    where TFilter : IExcelFilter<TRow>;
