namespace Ocelot.Modules;

[OcelotConfigGroups]
public class ModuleConfigGroup
{
    public string Id { get; init; } = "default";

    public string TranslationKey { get; init; } = "module_config_group.default";

    public int Priority { get; init; } = 0;

    public static readonly ModuleConfigGroup Default = new();

    public static readonly ModuleConfigGroup Ocelot = new() {
        Id = "ocelot",
        TranslationKey = "module_config_group.ocelot",
        Priority = -1000,
    };
}
