using System;
using System.Threading.Tasks;
using ECommons.DalamudServices;

namespace Ocelot.Chain;

public class FrameworkThreadLink : IChainlink
{
    private readonly Action action;

    public FrameworkThreadLink(Action action)
    {
        this.action = action;
    }

    public Task RunAsync(ChainContext context)
    {
        if (context.token.IsCancellationRequested)
            return Task.CompletedTask;

        var completion = new TaskCompletionSource();

        Svc.Framework.RunOnFrameworkThread(() =>
        {
            try
            {
                action();
                completion.SetResult();
            }
            catch (Exception ex)
            {
                completion.SetException(ex);
            }
        });

        return completion.Task;
    }
}
