using System.Reflection;
using Dalamud.Bindings.ImGui;
using Lumina.Excel;
using Ocelot.Config.Fields;
using Ocelot.Config.Renderers.Excel;
using Ocelot.Extensions;
using Ocelot.Services.Data;
using Ocelot.Services.Translation;

namespace Ocelot.Config.Renderers;

public class ExcelSelectRenderer<TRow, TDisplay, TFilter>(IDataRepository<TRow> data, TDisplay display, TFilter filter)
    : IFieldRenderer<ExcelSelectAttribute<TRow, TDisplay, TFilter>>
    where TRow : struct, IExcelRow<TRow>
    where TDisplay : IExcelDisplay<TRow>
    where TFilter : IExcelFilter<TRow>
{
    private sealed record CachedList(string[] Labels, uint[] Keys);

    private CachedList? cache = null;

    public bool Render(object target, PropertyInfo prop, ExcelSelectAttribute<TRow, TDisplay, TFilter> attr, Type owner, ITranslator translator)
    {
        if (cache == null)
        {
            var labels = new List<string>();
            var values = new List<uint>();

            foreach (var datum in data.GetAll())
            {
                if (!filter.Filter(datum))
                {
                    continue;
                }

                labels.Add(display.Display(datum));
                values.Add(datum.RowId);
            }

            cache = new CachedList(labels.ToArray(), values.ToArray());
        }


        if (prop.PropertyType != typeof(uint))
        {
            throw new InvalidOperationException(
                $"[ExcelSelectRenderer] can only be used on uint properties. {prop.DeclaringType?.Name}.{prop.Name} is {prop.PropertyType.Name}.");
        }

        var value = (uint)(prop.GetValue(target) ?? 0);

        var index = Array.IndexOf(cache.Keys, value);
        if (index < 0)
        {
            index = 0;
        }

        var label = prop.Label(owner, translator);
        var changed = ImGui.Combo(label, ref index, cache.Labels, cache.Labels.Length);

        prop.Tooltip(owner, translator);

        if (changed)
        {
            var selectedKey = cache.Keys[index];
            prop.SetValue(target, selectedKey);
        }

        return changed;
    }
}
