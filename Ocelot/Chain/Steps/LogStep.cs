using System;
using System.Threading;
using System.Threading.Tasks;
using ECommons.DalamudServices;

namespace Ocelot.Chain.Steps;

public class LogStep(Func<string> getLog, string name = "Info Log") : ActionStep(name)
{
    public LogStep(string log, string name = "Info Log") : this(() => log, name) { }

    protected virtual void Log(string message)
    {
        Svc.Log.Info(message);
    }

    protected override ValueTask<bool> Action(ChainContext context, CancellationToken token)
    {
        Log(getLog.Invoke());
        return ValueTask.FromResult(true);
    }
}

public class InfoLogStep : LogStep
{
    public InfoLogStep(string log, string name = "Info Log") : base(log, name) { }

    public InfoLogStep(Func<string> getLog, string name = "Info Log") : base(getLog, name) { }
}

public class DebugLogStep : LogStep
{
    public DebugLogStep(string log, string name = "Debug Log") : base(log, name) { }

    public DebugLogStep(Func<string> getLog, string name = "Debug Log") : base(getLog, name) { }

    protected override void Log(string message)
    {
        Svc.Log.Debug(message);
    }
}

public class ErrorLogStep : LogStep
{
    public ErrorLogStep(string log, string name = "Error Log") : base(log, name) { }

    public ErrorLogStep(Func<string> getLog, string name = "Error Log") : base(getLog, name) { }

    protected override void Log(string message)
    {
        Svc.Log.Error(message);
    }
}

public class FatalLogStep : LogStep
{
    public FatalLogStep(string log, string name = "Fatal Log") : base(log, name) { }

    public FatalLogStep(Func<string> getLog, string name = "Fatal Log") : base(getLog, name) { }

    protected override void Log(string message)
    {
        Svc.Log.Fatal(message);
    }
}

public class VerboseLogStep : LogStep
{
    public VerboseLogStep(string log, string name = "Verbose Log") : base(log, name) { }

    public VerboseLogStep(Func<string> getLog, string name = "Verbose Log") : base(getLog, name) { }

    protected override void Log(string message)
    {
        Svc.Log.Verbose(message);
    }
}

public class WarningLogStep : LogStep
{
    public WarningLogStep(string log, string name = "Warning Log") : base(log, name) { }

    public WarningLogStep(Func<string> getLog, string name = "Warning Log") : base(getLog, name) { }

    protected override void Log(string message)
    {
        Svc.Log.Warning(message);
    }
}
