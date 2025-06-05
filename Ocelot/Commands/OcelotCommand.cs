
using System;
using System.Collections.Generic;
using System.Linq;
using ECommons;

namespace Ocelot.Commands;

public abstract class OcelotCommand
{
    public abstract string command { get; }

    public abstract string description { get; }

    public virtual IReadOnlyList<string> aliases { get; set; } = [];

    public virtual IReadOnlyList<string> validArguments { get; set; } = [];

    public void Register()
    {
        EzCmd.Add(command, Handle, description);
        foreach (var alias in aliases)
        {
            EzCmd.Add(alias, Handle, $"Alias: {command}", int.MaxValue);
        }
    }

    private bool ValidateArguments(string arguments)
    {
        if (string.IsNullOrEmpty(arguments))
            return true;

        // If ValidArguments is empty, accept any argument by default
        if (validArguments.Count == 0)
            return true;

        // Check if the argument exactly matches any valid argument (case-insensitive)
        return validArguments.Any(arg => string.Equals(arg, arguments, StringComparison.OrdinalIgnoreCase));
    }

    public void Handle(string command, string arguments)
    {
        if (!ValidateArguments(arguments))
        {
            Logger.Error($"Invalid argument ({arguments}) to command ({command})");
        }

        Command(command, arguments);
    }

    public abstract void Command(string command, string arguments);
}
