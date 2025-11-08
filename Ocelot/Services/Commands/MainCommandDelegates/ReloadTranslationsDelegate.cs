namespace Ocelot.Services.Commands.MainCommandDelegates;

public class ReloadTranslationsDelegate(ReloadTranslationsCommand command) : IMainCommandDelegate
{
    public IOcelotCommand Command { get; } = command;
}
