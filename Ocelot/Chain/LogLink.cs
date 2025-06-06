using System;
using System.Threading.Tasks;
using ECommons.DalamudServices;

namespace Ocelot.Chain;

public class LogLink : IChainlink
{
    private readonly Func<string> messageFunc;

    public LogLink(string message)
        : this(() => message) { }

    public LogLink(Func<string> messageFunc)
    {
        this.messageFunc = messageFunc;
    }

    public Task RunAsync(ChainContext context)
    {
        Svc.Log.Debug(messageFunc());

        return Task.CompletedTask;
    }
}
