using System.Reflection;
using Dalamud.Bindings.ImGui;
using Ocelot.Config.Fields;

namespace Ocelot.Config.Renderers;

public sealed class FloatRangeRenderer : IFieldRenderer<FloatRangeAttribute>
{
    public bool Render(object target, PropertyInfo prop, FloatRangeAttribute attr)
    {
        if (prop.PropertyType != typeof(float))
        {
            throw new InvalidOperationException(
                $"[FloatRange] can only be used on float properties. {prop.DeclaringType?.Name}.{prop.Name} is {prop.PropertyType.Name}.");
        }

        var label = prop.Name;
        var value = (float)(prop.GetValue(target) ?? 0f);

        var changed = ImGui.SliderFloat(label, ref value, attr.Min, attr.Max);

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
