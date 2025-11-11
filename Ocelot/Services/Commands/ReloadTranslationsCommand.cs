using Ocelot.Services.Translation;

namespace Ocelot.Services.Commands;

public class ReloadTranslationsCommand(ITranslationRepository translations, ITranslator<ReloadTranslationsCommand> translator) : OcelotCommand(translator)
{
    public override string Command { get; } = "reload-translations";

    public override List<string> Aliases { get; } = ["reload-translation", "rt"];

    public override void Execute(CommandContext _)
    {
        translations.Reload();
    }
}
