using Ocelot.Config.Attributes;

namespace Ocelot.Modules;

public class OcelotCoreConfig : ModuleConfig
{
    [List(typeof(string), nameof(LanguageProvider))]
    public string Language { get; set; } = "";

    public string? SelectedConfigModuleId { get; set; }
}
