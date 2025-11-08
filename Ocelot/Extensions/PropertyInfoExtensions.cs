using System.Reflection;
using Dalamud.Bindings.ImGui;
using Ocelot.Services.Translation;

namespace Ocelot.Extensions;

public static class PropertyInfoExtensions
{
    private static string GetFieldKeyBase(this PropertyInfo prop, Type owner)
    {
        var config = owner.Name.Replace("Config", "").ToSnakeCase();
        var field = prop.Name.ToSnakeCase();

        return $"config.{config}.fields.{field}";
    }

    public static string GetFieldLabelKey(this PropertyInfo prop, Type owner)
    {
        return $"{prop.GetFieldKeyBase(owner)}.label";
    }

    public static string GetFieldTooltipKey(this PropertyInfo prop, Type owner)
    {
        return $"{prop.GetFieldKeyBase(owner)}.tooltip";
    }

    public static string Label(this PropertyInfo prop, Type owner, ITranslator translator)
    {
        var key = prop.GetFieldLabelKey(owner);
        return translator.T(key);
    }

    public static void Tooltip(this PropertyInfo prop, Type owner, ITranslator translator)
    {
        var tooltipKey = prop.GetFieldTooltipKey(owner);
        if (translator.Has(tooltipKey) && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(translator.T(tooltipKey));
        }
    }
}
