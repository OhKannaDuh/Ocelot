using System.Collections.Generic;
using ECommons.DalamudServices;

namespace Ocelot.Commands;

public class ConfigOcelotCommand : OcelotCommand
{
    public override string Command { get; init; } = $"/{Svc.PluginInterface.InternalName.ToLower()}cfg";

    public override string Description { get; init; } = "";

    public override void Execute(List<string> arguments)
    {
        OcelotPlugin.Plugin.Windows.ToggleConfigUI();
    }
}
