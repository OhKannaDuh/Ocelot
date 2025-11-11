using Dalamud.Plugin.Services;

namespace Ocelot.Services.Logger;

public class PluginLogger(IPluginLog logger) : ILogger
{
    public void Log(string format, params object[] args)
    {
        Info(format, args);
    }

    public void Info(string format, params object[] args)
    {
        logger.Info(format, args);
    }

    public void Warn(string format, params object[] args)
    {
        Warning(format, args);
    }

    public void Warning(string format, params object[] args)
    {
        logger.Warning(format, args);
    }

    public void Error(string format, params object[] args)
    {
        logger.Error(format, args);
    }

    public void Debug(string format, params object[] args)
    {
        logger.Debug(format, args);
    }

    public void Fatal(string format, params object[] args)
    {
        logger.Fatal(format, args);
    }

    public void Verbose(string format, params object[] args)
    {
        logger.Verbose(format, args);
    }

    public void Fatal(Exception? exception, string messageTemplate, params object[] values)
    {
        logger.Fatal(exception, messageTemplate, values);
    }

    public void Error(Exception? exception, string messageTemplate, params object[] values)
    {
        logger.Error(exception, messageTemplate, values);
    }

    public void Warn(Exception? exception, string messageTemplate, params object[] values)
    {
        Warning(exception, messageTemplate, values);
    }

    public void Warning(Exception? exception, string messageTemplate, params object[] values)
    {
        logger.Warning(exception, messageTemplate, values);
    }

    public void Info(Exception? exception, string messageTemplate, params object[] values)
    {
        logger.Info(exception, messageTemplate, values);
    }

    public void Debug(Exception? exception, string messageTemplate, params object[] values)
    {
        logger.Debug(exception, messageTemplate, values);
    }

    public void Verbose(Exception? exception, string messageTemplate, params object[] values)
    {
        logger.Verbose(exception, messageTemplate, values);
    }
}
