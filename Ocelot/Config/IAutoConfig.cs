using Dalamud.Bindings.ImGui;
using Ocelot.Extensions;
using Ocelot.Services.Translation;

namespace Ocelot.Config;

public interface IAutoConfig
{
    string GetLabelKey()
    {
        var name = GetType().Name.Replace("Config", "").ToSnakeCase();

        return $"config.{name}.label";
    }

    string GetTooltipKey()
    {
        var name = GetType().Name.Replace("Config", "").ToSnakeCase();

        return $"config.{name}.tooltip";
    }

    string Label(ITranslator translator)
    {
        return translator.T(GetLabelKey());
    }

    void Tooltip(ITranslator translator)
    {
        var key = GetTooltipKey();
        if (translator.Has(key) && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(translator.T(key));
        }
    }
}
