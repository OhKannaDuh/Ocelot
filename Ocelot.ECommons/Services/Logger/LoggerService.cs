using ECommons.DalamudServices;
using Ocelot.Services.Logger;

namespace Ocelot.ECommons.Services.Logger;

internal sealed class ECommonsLoggerService : ILoggerService
{
    public void Log(string format, params object[] args)
    {
        Info(format, args);
    }

    public void Info(string format, params object[] args)
    {
        Svc.Log.Information(format, args);
    }

    public void Warn(string format, params object[] args)
    {
        Svc.Log.Warning(format, args);
    }

    public void Warning(string format, params object[] args)
    {
        Svc.Log.Warning(format, args);
    }

    public void Error(string format, params object[] args)
    {
        Svc.Log.Error(format, args);
    }

    public void Debug(string format, params object[] args)
    {
        Svc.Log.Debug(format, args);
    }

    public void Fatal(string format, params object[] args)
    {
        Svc.Log.Fatal(format, args);
    }

    public void Verbose(string format, params object[] args)
    {
        Svc.Log.Verbose(format, args);
    }
}
