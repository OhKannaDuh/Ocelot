using System;
using System.Collections.Generic;
using System.Linq;
using ECommons;

namespace Ocelot.Commands;

public abstract class OcelotCommand
{
    public abstract string Command { get; init; }

    public abstract string Description { get; init; }

    public virtual IReadOnlyList<string> Aliases { get; init; } = [];

    public virtual IReadOnlyList<string> ValidArguments { get; init; } = [];

    public void Register()
    {
        var cmd = Command.StartsWith('/') ? Command : $"/{Command}";
        EzCmd.Add(cmd, Handle, Description);
        foreach (var alias in Aliases)
        {
            EzCmd.Add(alias, Handle, $"Alias: {cmd}", int.MaxValue);
        }
    }

    private bool ValidateArguments(string arguments)
    {
        if (string.IsNullOrEmpty(arguments))
        {
            return true;
        }

        return ValidArguments.Count == 0 || ValidArguments.Any(arg => string.Equals(arg, arguments, StringComparison.OrdinalIgnoreCase));
    }

    public void Handle(string command, string arguments)
    {
        arguments = arguments.ToLower().Trim();

        if (!ValidateArguments(arguments))
        {
            Logger.Error($"Invalid argument ({arguments}) to command ({command})");
        }

        Execute(arguments.Split(" ").ToList());
    }

    public abstract void Execute(List<string> arguments);
}
