using System.Collections.Concurrent;
using Ocelot.Lifecycle;
using Ocelot.Services.Logger;

namespace Ocelot.Chain;

public sealed record ActiveChainRun(string Name, CancellationTokenSource Cts, Task<ChainResult> Task);

public class ChainManager(
    IChainFactory chains,
    IServiceProvider services,
    ILogger<ChainManager> logger
) : IChainManager, IOnUpdate
{
    private readonly ConcurrentDictionary<Guid, ActiveChainRun> active = new();

    public Task<ChainResult> ExecuteAsync(Func<IChainFactory, IChain> factory)
    {
        var chain = factory(chains);
        var cts = new CancellationTokenSource();
        var ctx = new ChainContext(chain.Name, services, cts.Token);
        var task = chain.ExecuteAsync(ctx);

        active[ctx.RunId] = new ActiveChainRun(chain.Name, cts, task);

        return task;
    }

    public Task<ChainResult> Manage(IChain chain)
    {
        var cts = new CancellationTokenSource();
        var ctx = new ChainContext(chain.Name, services, cts.Token);
        var task = chain.ExecuteAsync(ctx);

        active[ctx.RunId] = new ActiveChainRun(chain.Name, cts, task);

        return task;
    }

    public void CancelAll()
    {
        foreach (var run in active.Values)
        {
            logger.Debug("Cancelling task {0}", run.Name);
            run.Cts.Cancel();
        }
    }

    public void Update()
    {
        foreach (var (id, run) in active.ToArray())
        {
            if (!run.Task.IsCompleted)
            {
                continue;
            }

            _ = run.Task.Exception;

            run.Cts.Dispose();
            active.TryRemove(id, out _);
        }
    }
}
