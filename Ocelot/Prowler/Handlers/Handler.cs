using Ocelot.Services;
using Ocelot.Services.Logger;
using Ocelot.States;

namespace Ocelot.Prowler.Handlers;

public abstract class Handler : StateHandler<ProwlState, Prowl>
{
    private static ILoggerService Logger {
        get => OcelotServices.GetCached<ILoggerService>();
    }

    protected void LogDebug(string format, params object[] args)
    {
        Logger.Debug($"[Prowler] {format}", args);
    }

    public override ProwlState? Handle(Prowl? prowl = null)
    {
        if (prowl == null)
        {
            return ProwlState.Faulted;
        }

        return null;
    }
}
