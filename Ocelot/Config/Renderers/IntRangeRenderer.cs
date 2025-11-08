using System.Reflection;
using Dalamud.Bindings.ImGui;
using Ocelot.Config.Fields;
using Ocelot.Extensions;
using Ocelot.Services.Translation;

namespace Ocelot.Config.Renderers;

public sealed class IntRangeRenderer : IFieldRenderer<IntRangeAttribute>
{
    public bool Render(object target, PropertyInfo prop, IntRangeAttribute attr, Type owner, ITranslator translator)
    {
        if (prop.PropertyType != typeof(int))
        {
            throw new InvalidOperationException(
                $"[FloatRange] can only be used on float properties. {prop.DeclaringType?.Name}.{prop.Name} is {prop.PropertyType.Name}.");
        }

        var value = (int)(prop.GetValue(target) ?? 0);

        var changed = ImGui.SliderInt(prop.Label(owner, translator), ref value, attr.Min, attr.Max);

        prop.Tooltip(owner, translator);

        if (changed)
        {
            prop.SetValue(target, value);
        }

        return changed;
    }
}
