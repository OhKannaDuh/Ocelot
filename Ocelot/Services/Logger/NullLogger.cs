namespace Ocelot.Services.Logger;

public class NullLogger : ILogger
{
    public void Log(string format, params object[] args)
    {
    }

    public void Info(string format, params object[] args)
    {
    }

    public void Warn(string format, params object[] args)
    {
    }

    public void Warning(string format, params object[] args)
    {
    }

    public void Error(string format, params object[] args)
    {
    }

    public void Debug(string format, params object[] args)
    {
    }

    public void Fatal(string format, params object[] args)
    {
    }

    public void Verbose(string format, params object[] args)
    {
    }

    public void Fatal(Exception? exception, string messageTemplate, params object[] values)
    {
    }

    public void Error(Exception? exception, string messageTemplate, params object[] values)
    {
    }

    public void Warn(Exception? exception, string messageTemplate, params object[] values)
    {
    }

    public void Warning(Exception? exception, string messageTemplate, params object[] values)
    {
    }

    public void Info(Exception? exception, string messageTemplate, params object[] values)
    {
    }

    public void Debug(Exception? exception, string messageTemplate, params object[] values)
    {
    }

    public void Verbose(Exception? exception, string messageTemplate, params object[] values)
    {
    }
}
