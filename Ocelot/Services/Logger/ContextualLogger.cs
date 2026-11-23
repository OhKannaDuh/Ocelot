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
        inner.Log(Context + " " + format, Readable(args));
    }

    public void Info(string format, params object[] args)
    {
        inner.Info(Context + " " + format, Readable(args));
    }

    public void Warn(string format, params object[] args)
    {
        inner.Warn(Context + " " + format, Readable(args));
    }

    public void Warning(string format, params object[] args)
    {
        inner.Warning(Context + " " + format, Readable(args));
    }

    public void Error(string format, params object[] args)
    {
        inner.Error(Context + " " + format, Readable(args));
    }

    public void Debug(string format, params object[] args)
    {
        inner.Debug(Context + " " + format, Readable(args));
    }

    public void Fatal(string format, params object[] args)
    {
        inner.Fatal(Context + " " + format, Readable(args));
    }

    public void Verbose(string format, params object[] args)
    {
        inner.Verbose(Context + " " + format, Readable(args));
    }

    public void Fatal(Exception? exception, string format, params object[] args)
    {
        inner.Fatal(exception, Context + " " + format, Readable(args));
    }

    public void Error(Exception? exception, string format, params object[] args)
    {
        inner.Error(exception, Context + " " + format, Readable(args));
    }

    public void Warn(Exception? exception, string format, params object[] args)
    {
        inner.Warn(exception, Context + " " + format, Readable(args));
    }

    public void Warning(Exception? exception, string format, params object[] args)
    {
        inner.Warning(exception, Context + " " + format, Readable(args));
    }

    public void Info(Exception? exception, string format, params object[] args)
    {
        inner.Info(exception, Context + " " + format, Readable(args));
    }

    public void Debug(Exception? exception, string format, params object[] args)
    {
        inner.Debug(exception, Context + " " + format, Readable(args));
    }

    public void Verbose(Exception? exception, string format, params object[] args)
    {
        inner.Verbose(exception, Context + " " + format, Readable(args));
    }

    // Vectors print with full precision in the PC's number format ("<-181,43318. 29,01438. ...>"); whole yalms read better.
    private static object[] Readable(object[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] is System.Numerics.Vector3 v)
            {
                args[i] = string.Create(
                    System.Globalization.CultureInfo.InvariantCulture,
                    $"({v.X:F0}, {v.Y:F0}, {v.Z:F0})");
            }
        }

        return args;
    }
}

