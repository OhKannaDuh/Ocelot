namespace Ocelot.Services.Commands;

public interface IOcelotCommand
{
    string Command { get; }

    List<string> Aliases { get; }

    string HelpTranslationKey { get; }

    int DisplayOrder { get; }

    bool Hidden { get; }

    bool IsEnabled { get; }

    bool ValidateArguments(string[] args, out string? errorMessage);

    (string help, bool show) BuildHelp();

    void Execute(CommandContext context);
}

public sealed class CommandContext
{
    public string RawArgs { get; init; } = string.Empty;

    public string[] Args { get; init; } = [];

    public CommandContext Skip(int count = 1)
    {
        return new CommandContext
        {
            Args = Args.Skip(count).ToArray(),
            RawArgs = string.Join(" ", Args.Skip(count)),
        };
    }
}
