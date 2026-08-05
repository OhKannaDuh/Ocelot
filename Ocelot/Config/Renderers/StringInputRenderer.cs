using System.Reflection;
using Dalamud.Bindings.ImGui;
using Ocelot.Config.Fields;
using Ocelot.Extensions;
using Ocelot.Services.Translation;

namespace Ocelot.Config.Renderers;

public sealed class StringInputRenderer : IFieldRenderer<StringInputAttribute>
{
    public bool Render(object target, PropertyInfo prop, StringInputAttribute attr, Type owner, ITranslator translator)
    {
        if (prop.PropertyType != typeof(string))
        {
            throw new InvalidOperationException(
                $"[StringInput] can only be used on string properties. {prop.DeclaringType?.Name}.{prop.Name} is {prop.PropertyType.Name}.");
        }

        string value = (string?)prop.GetValue(target) ?? string.Empty;
        bool changed = ImGui.InputTextWithHint(
            prop.Label(owner, translator),
            "https://…/api/v1/observations",
            ref value,
            attr.MaxLength);
        prop.Tooltip(owner, translator);

        if (changed)
        {
            prop.SetValue(target, value);
        }

        return changed;
    }
}
