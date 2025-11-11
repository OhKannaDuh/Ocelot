namespace Ocelot.Services.Logger;

public class ContextualLogger<TScope>(ILogger inner) : ILogger<TScope>
{
    private bool IsOcelot
    {
        get => typeof(TScope).FullName!.StartsWith("Ocelot");
    }

    private string Name
    {
        get => typeof(TScope).Name.Split('`')[0];
    }

    private string Context
    {
        get => IsOcelot ? $"[Ocelot] [{Name}]" : $"[{Name}]";
    }

    public void Log(string format, params object[] args)
    {
        inner.Log(Context + " " + format, args);
    }

    public void Info(string format, params object[] args)
    {
        inner.Info(Context + " " + format, args);
    }

    public void Warn(string format, params object[] args)
    {
        inner.Warn(Context + " " + format, args);
    }

    public void Warning(string format, params object[] args)
    {
        inner.Warning(Context + " " + format, args);
    }

    public void Error(string format, params object[] args)
    {
        inner.Error(Context + " " + format, args);
    }

    public void Debug(string format, params object[] args)
    {
        inner.Debug(Context + " " + format, args);
    }

    public void Fatal(string format, params object[] args)
    {
        inner.Fatal(Context + " " + format, args);
    }

    public void Verbose(string format, params object[] args)
    {
        inner.Verbose(Context + " " + format, args);
    }

    public void Fatal(Exception? exception, string format, params object[] args)
    {
        inner.Fatal(exception, Context + " " + format, args);
    }

    public void Error(Exception? exception, string format, params object[] args)
    {
        inner.Error(Context + " " + format, args);
    }

    public void Warn(Exception? exception, string format, params object[] args)
    {
        inner.Warn(Context + " " + format, args);
    }

    public void Warning(Exception? exception, string format, params object[] args)
    {
        inner.Warning(Context + " " + format, args);
    }

    public void Info(Exception? exception, string format, params object[] args)
    {
        inner.Info(Context + " " + format, args);
    }

    public void Debug(Exception? exception, string format, params object[] args)
    {
        inner.Debug(Context + " " + format, args);
    }

    public void Verbose(Exception? exception, string format, params object[] args)
    {
        inner.Verbose(Context + " " + format, args);
    }
}
