using System.Reflection;
using Dalamud.Bindings.ImGui;
using Ocelot.Config.Fields;
using Ocelot.Extensions;
using Ocelot.Services.Translation;

namespace Ocelot.Config.Renderers;

public sealed class CheckboxRenderer : IFieldRenderer<CheckboxAttribute>
{
    public bool Render(object target, PropertyInfo prop, CheckboxAttribute attr, Type owner, ITranslator translator)
    {
        if (prop.PropertyType != typeof(bool))
        {
            throw new InvalidOperationException(
                $"[Checkbox] can only be used on bool properties. {prop.DeclaringType?.Name}.{prop.Name} is {prop.PropertyType.Name}.");
        }

        var value = (bool)(prop.GetValue(target) ?? false);

        var changed = ImGui.Checkbox(prop.Label(owner, translator), ref value);

        prop.Tooltip(owner, translator);

        if (changed)
        {
            prop.SetValue(target, value);
        }

        return changed;
    }
}
