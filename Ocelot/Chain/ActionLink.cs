using System;
using System.Threading.Tasks;

namespace Ocelot.Chain;

public class ActionLink : IChainlink
{
    private readonly Func<Task> _action;

    public ActionLink(Action action)
    {
        _action = () => {
            action();
            return Task.CompletedTask;
        };
    }

    public ActionLink(Func<Task> asyncAction)
    {
        _action = asyncAction;
    }

    public async Task RunAsync(ChainContext context)
    {
        if (context.token.IsCancellationRequested)
            return;

        await _action();
    }
}
