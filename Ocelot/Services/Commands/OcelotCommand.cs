using Ocelot.Services.Translation;

namespace Ocelot.Services.Commands;

public abstract class OcelotCommand(ITranslator translator) : IOcelotCommand
{
    public abstract string Command { get; }

    public virtual List<string> Aliases
    {
        get => [];
    }

    public virtual string HelpTranslationKey { get; } = "";

    public virtual int DisplayOrder
    {
        get => -1;
    }

    public virtual bool Hidden
    {
        get => false;
    }

    public virtual bool IsEnabled
    {
        get => true;
    }

    public virtual bool ValidateArguments(string[] args, out string? errorMessage)
    {
        errorMessage = null;
        return true;
    }

    public virtual (string help, bool show) BuildHelp()
    {
        return !translator.Has(HelpTranslationKey) ? (string.Empty, false) : (translator.T(HelpTranslationKey), true);
    }

    public abstract void Execute(CommandContext context);
}
