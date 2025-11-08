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
}
