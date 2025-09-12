namespace Ocelot.Services.Logger;

public interface ILoggerService
{
    void Log(string format, params object[] args);

    void Info(string format, params object[] args);

    void Warn(string format, params object[] args);

    void Warning(string format, params object[] args);

    void Error(string format, params object[] args);

    void Debug(string format, params object[] args);

    void Fatal(string format, params object[] args);

    void Verbose(string format, params object[] args);
}
