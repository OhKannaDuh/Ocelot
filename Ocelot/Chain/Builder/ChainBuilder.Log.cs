using System;
using Ocelot.Chain.Steps;

namespace Ocelot.Chain.Builder;

public sealed partial class ChainBuilder
{
    public ChainBuilder Log(Func<string> getLog, string? name = null)
    {
        return Then(name == null ? new LogStep(getLog) : new LogStep(getLog, name));
    }

    public ChainBuilder Log(string log, string? name = null)
    {
        return Then(name == null ? new LogStep(log) : new LogStep(log, name));
    }

    public ChainBuilder Info(Func<string> getLog, string? name = null)
    {
        return Then(name == null ? new InfoLogStep(getLog) : new InfoLogStep(getLog, name));
    }

    public ChainBuilder Info(string log, string? name = null)
    {
        return Then(name == null ? new InfoLogStep(log) : new InfoLogStep(log, name));
    }

    public ChainBuilder Debug(Func<string> getLog, string? name = null)
    {
        return Then(name == null ? new DebugLogStep(getLog) : new DebugLogStep(getLog, name));
    }

    public ChainBuilder Debug(string log, string? name = null)
    {
        return Then(name == null ? new DebugLogStep(log) : new DebugLogStep(log, name));
    }

    public ChainBuilder Error(Func<string> getLog, string? name = null)
    {
        return Then(name == null ? new ErrorLogStep(getLog) : new ErrorLogStep(getLog, name));
    }

    public ChainBuilder Error(string log, string? name = null)
    {
        return Then(name == null ? new ErrorLogStep(log) : new ErrorLogStep(log, name));
    }

    public ChainBuilder Fatal(Func<string> getLog, string? name = null)
    {
        return Then(name == null ? new FatalLogStep(getLog) : new FatalLogStep(getLog, name));
    }

    public ChainBuilder Fatal(string log, string? name = null)
    {
        return Then(name == null ? new FatalLogStep(log) : new FatalLogStep(log, name));
    }

    public ChainBuilder Verbose(Func<string> getLog, string? name = null)
    {
        return Then(name == null ? new VerboseLogStep(getLog) : new VerboseLogStep(getLog, name));
    }

    public ChainBuilder Verbose(string log, string? name = null)
    {
        return Then(name == null ? new VerboseLogStep(log) : new VerboseLogStep(log, name));
    }

    public ChainBuilder Warning(Func<string> getLog, string? name = null)
    {
        return Then(name == null ? new WarningLogStep(getLog) : new WarningLogStep(getLog, name));
    }

    public ChainBuilder Warning(string log, string? name = null)
    {
        return Then(name == null ? new WarningLogStep(log) : new WarningLogStep(log, name));
    }
}
