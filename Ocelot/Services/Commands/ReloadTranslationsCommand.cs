using Ocelot.Services.Translation;

namespace Ocelot.Services.Commands;

public class ReloadTranslationsCommand(ITranslationRepository translations, ITranslator translator) : OcelotCommand(translator)
{
    public override string Command { get; } = "reload-languages";

    public override List<string> Aliases { get; } = ["reload-language", "rt"];

    public override string HelpTranslationKey { get; } = "commands.reload-languages.help";

    public override void Execute(CommandContext _)
    {
        translations.Reload();
    }
}
