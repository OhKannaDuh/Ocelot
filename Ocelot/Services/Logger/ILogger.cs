namespace Ocelot.Services.Logger;

public interface ILogger
{
    void Log(string format, params object[] args);

    void Info(string format, params object[] args);

    void Warn(string format, params object[] args);

    void Warning(string format, params object[] args);

    void Error(string format, params object[] args);

    void Debug(string format, params object[] args);

    void Fatal(string format, params object[] args);

    void Verbose(string format, params object[] args);

    void Fatal(Exception? exception, string messageTemplate, params object[] values);

    void Error(Exception? exception, string messageTemplate, params object[] values);

    void Warn(Exception? exception, string messageTemplate, params object[] values);

    void Warning(Exception? exception, string messageTemplate, params object[] values);

    void Info(Exception? exception, string messageTemplate, params object[] values);

    void Debug(Exception? exception, string messageTemplate, params object[] values);

    void Verbose(Exception? exception, string messageTemplate, params object[] values);
}

public interface ILogger<out TScope> : ILogger;
