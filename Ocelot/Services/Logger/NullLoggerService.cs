namespace Ocelot.Services.Logger;

public class NullLoggerService : ILoggerService
{
    public void Log(string format, params object[] args) { }

    public void Info(string format, params object[] args) { }

    public void Warn(string format, params object[] args) { }

    public void Warning(string format, params object[] args) { }

    public void Error(string format, params object[] args) { }

    public void Debug(string format, params object[] args) { }

    public void Fatal(string format, params object[] args) { }

    public void Verbose(string format, params object[] args) { }
}
