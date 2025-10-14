using System.Reflection;
using Dalamud.Bindings.ImGui;
using Ocelot.Config.Fields;

namespace Ocelot.Config.Renderers;

public sealed class CheckboxRenderer : IFieldRenderer<CheckboxAttribute>
{
    public bool Render(object target, PropertyInfo prop, CheckboxAttribute attr)
    {
        if (prop.PropertyType != typeof(bool))
        {
            throw new InvalidOperationException(
                $"[Checkbox] can only be used on bool properties. {prop.DeclaringType?.Name}.{prop.Name} is {prop.PropertyType.Name}.");
        }

        // var label = attr.Label ?? prop.Name;
        var label = prop.Name;
        var value = (bool)(prop.GetValue(target) ?? false);

        var changed = ImGui.Checkbox(label, ref value);

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
