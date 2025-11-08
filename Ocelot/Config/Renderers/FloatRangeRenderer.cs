using System.Reflection;
using Dalamud.Bindings.ImGui;
using Ocelot.Config.Fields;
using Ocelot.Extensions;
using Ocelot.Services.Translation;

namespace Ocelot.Config.Renderers;

public sealed class FloatRangeRenderer : IFieldRenderer<FloatRangeAttribute>
{
    public bool Render(object target, PropertyInfo prop, FloatRangeAttribute attr, Type owner, ITranslator translator)
    {
        if (prop.PropertyType != typeof(float))
        {
            throw new InvalidOperationException(
                $"[FloatRange] can only be used on float properties. {prop.DeclaringType?.Name}.{prop.Name} is {prop.PropertyType.Name}.");
        }

        var value = (float)(prop.GetValue(target) ?? 0f);

        var changed = ImGui.SliderFloat(prop.Label(owner, translator), ref value, attr.Min, attr.Max);

        prop.Tooltip(owner, translator);

        if (changed)
        {
            prop.SetValue(target, value);
        }

        return changed;
    }
}
