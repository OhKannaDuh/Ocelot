using System.Collections.Generic;
using ECommons.DalamudServices;
using Ocelot.Services;
using Ocelot.Services.Windows;

namespace Ocelot.Commands;

public class ConfigOcelotCommand : OcelotCommand
{
    public override string Command { get; init; } = $"/{Svc.PluginInterface.InternalName.ToLower()}cfg";

    public override string Description { get; init; } = "";

    public override void Execute(List<string> arguments)
    {
        OcelotServices.GetCached<IWindowManager>().ToggleConfigUI();
    }
}
