namespace Ocelot.Services.Commands;

public interface IMainCommandDelegate
{
    IOcelotCommand Command { get; }
}
