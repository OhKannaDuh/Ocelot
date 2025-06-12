using System;
using ECommons.Automation.NeoTaskManager;
using ECommons.Throttlers;

namespace Ocelot.Chain;

public abstract class RetryChainFactory : ChainFactory
{
    public override Func<Chain> Factory()
    {
        var name = GetType().Name;
        return () => Chain.Create(name)
            .Then(new TaskManagerTask(() =>
            {
                var watcher = ChainManager.Get($"{name}##watcher");
                if (!watcher.IsRunning)
                {
                    if (EzThrottler.Throttle(name, GetThrottle()))
                    {
                        watcher.Submit(() => Create(Chain.Create(name)));
                    }
                }

                return IsComplete();
            }, new() { TimeLimitMS = GetTimeout() }));
    }

    public abstract bool IsComplete();

    public virtual int GetThrottle() => 500;

    public virtual int GetTimeout() => 10000;

    public override TaskManagerConfiguration? Config() => new()
    {
        TimeLimitMS = int.MaxValue
    };
}