using System.Reflection;
using Dalamud.Bindings.ImGui;
using Ocelot.Config.Fields;

namespace Ocelot.Config.Renderers;

public sealed class IntRangeRenderer : IFieldRenderer<IntRangeAttribute>
{
    public bool Render(object target, PropertyInfo prop, IntRangeAttribute attr)
    {
        if (prop.PropertyType != typeof(int))
        {
            throw new InvalidOperationException(
                $"[FloatRange] can only be used on float properties. {prop.DeclaringType?.Name}.{prop.Name} is {prop.PropertyType.Name}.");
        }

        var label = prop.Name;
        var value = (int)(prop.GetValue(target) ?? 0);

        var changed = ImGui.SliderInt(label, ref value, attr.Min, attr.Max);

        // if (!string.IsNullOrEmpty(attr.Tooltip) && ImGui.IsItemHovered())
        // {
        //     ImGui.SetTooltip(attr.Tooltip);
        // }

        if (changed)
        {
            prop.SetValue(target, value);
        }

        return changed;
    }
}
