using System;
using System.Collections.Generic;
using System.Linq;
using Ocelot.Modules;

namespace Ocelot.Commands;

public class CommandManager : IDisposable
{
    private readonly List<OcelotCommand> commands = new();

    public MainOcelotCommand? Main { get; private set; } = null;

    public ConfigOcelotCommand? Config { get; private set; } = null;

    public void Initialize(OcelotPlugin plugin)
    {
        var commandTypes = Registry.GetTypesWithAttribute<OcelotCommandAttribute>()
            .Where(t => typeof(OcelotCommand).IsAssignableFrom(t))
            .ToList();

        foreach (var type in commandTypes)
        {
            Logger.Info($"Registering command: {type.FullName}");
            var instance = (OcelotCommand)Activator.CreateInstance(type, plugin)!;
            commands.Add(instance);
            instance.Register();
        }
    }

    public void InitializeMainCommand(
        string command,
        string description = "",
        IReadOnlyList<string>? aliases = null,
        IReadOnlyList<string>? validArguments = null)
    {
        Main = new MainOcelotCommand {
            Command = command,
            Description = description,
            Aliases = aliases ?? [],
            ValidArguments = validArguments ?? [],
        };

        Main.Register();
    }

    public void InitializeConfigCommand(
        string command,
        string description = "",
        IReadOnlyList<string>? aliases = null,
        IReadOnlyList<string>? validArguments = null)
    {
        Config = new ConfigOcelotCommand {
            Command = command,
            Description = description,
            Aliases = aliases ?? [],
            ValidArguments = validArguments ?? [],
        };

        commands.Add(Config);
        Config.Register();
    }

    public void Dispose()
    {
        Main = null;
        Config = null;
    }
}
