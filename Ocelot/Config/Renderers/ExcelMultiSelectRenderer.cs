using System.Reflection;
using Dalamud.Bindings.ImGui;
using Lumina.Excel;
using Ocelot.Config.Fields;
using Ocelot.Config.Renderers.Excel;
using Ocelot.Extensions;
using Ocelot.Services.Data;
using Ocelot.Services.Translation;

namespace Ocelot.Config.Renderers;

public class ExcelMultiSelectRenderer<TRow, TDisplay, TFilter>(IDataRepository<TRow> data, TDisplay display, TFilter filter)
    : IFieldRenderer<ExcelMultiSelectAttribute<TRow, TDisplay, TFilter>>
    where TRow : struct, IExcelRow<TRow>
    where TDisplay : IExcelDisplay<TRow>
    where TFilter : IExcelFilter<TRow>
{
    private sealed record CachedList(string[] Labels, uint[] Keys);

    private CachedList? cache = null;

    public bool Render(object target, PropertyInfo prop, ExcelMultiSelectAttribute<TRow, TDisplay, TFilter> attr, Type owner, ITranslator translator)
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

        var propType = prop.PropertyType;
        if (propType != typeof(List<uint>) && propType != typeof(HashSet<uint>) && propType != typeof(uint[]))
        {
            throw new InvalidOperationException(
                $"[ExcelMultiSelectRenderer] must be used on List<uint>, HashSet<uint>, or uint[] properties. " +
                $"{prop.DeclaringType?.Name}.{prop.Name} is {propType.Name}.");
        }

        HashSet<uint> workingSet;
        var created = false;

        if (propType == typeof(HashSet<uint>))
        {
            workingSet = (HashSet<uint>?)prop.GetValue(target) ?? new HashSet<uint>();
            if (prop.GetValue(target) == null)
            {
                prop.SetValue(target, workingSet);
                created = true;
            }
        }
        else if (propType == typeof(List<uint>))
        {
            var list = (List<uint>?)prop.GetValue(target) ?? new List<uint>();
            if (prop.GetValue(target) == null)
            {
                prop.SetValue(target, list);
                created = true;
            }

            workingSet = new HashSet<uint>(list);
        }
        else // uint[]
        {
            var arr = (uint[]?)prop.GetValue(target) ?? Array.Empty<uint>();
            workingSet = new HashSet<uint>(arr);
            if (prop.GetValue(target) == null)
            {
                created = true;
            }
        }

        var label = prop.Label(owner, translator);

        string PreviewText()
        {
            if (workingSet.Count == 0)
            {
                return translator.T("None");
            }

            var selectedLabels = new List<string>();
            for (var i = 0; i < cache!.Keys.Length && selectedLabels.Count < 3; i++)
            {
                if (workingSet.Contains(cache.Keys[i]))
                {
                    selectedLabels.Add(cache.Labels[i]);
                }
            }

            if (workingSet.Count <= 3)
            {
                return string.Join(", ", selectedLabels);
            }

            return $"{string.Join(", ", selectedLabels)}, +{workingSet.Count - 3}";
        }

        var changed = created;

        if (ImGui.BeginCombo(label, PreviewText()))
        {
            for (var i = 0; i < cache!.Labels.Length; i++)
            {
                var key = cache.Keys[i];
                var selected = workingSet.Contains(key);

                if (selected)
                {
                    if (ImGui.Selectable(cache.Labels[i], selected, ImGuiSelectableFlags.DontClosePopups))
                    {
                        if (selected)
                        {
                            workingSet.Remove(key);
                        }
                        else
                        {
                            workingSet.Add(key);
                        }

                        changed = true;
                    }
                }
            }


            for (var i = 0; i < cache!.Labels.Length; i++)
            {
                var key = cache.Keys[i];
                var selected = workingSet.Contains(key);

                if (!selected)
                {
                    if (ImGui.Selectable(cache.Labels[i], selected, ImGuiSelectableFlags.DontClosePopups))
                    {
                        if (selected)
                        {
                            workingSet.Remove(key);
                        }
                        else
                        {
                            workingSet.Add(key);
                        }

                        changed = true;
                    }
                }
            }

            ImGui.EndCombo();
        }

        prop.Tooltip(owner, translator);

        if (changed)
        {
            if (propType == typeof(HashSet<uint>))
            {
                prop.SetValue(target, workingSet);
            }
            else if (propType == typeof(List<uint>))
            {
                prop.SetValue(target, workingSet.ToList());
            }
            else // uint[]
            {
                prop.SetValue(target, workingSet.ToArray());
            }
        }

        return changed;
    }
}
