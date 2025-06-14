using System;
using ECommons.Automation.NeoTaskManager;
using ECommons.Throttlers;

namespace Ocelot.Chain;

public abstract class RetryChainFactory : ChainFactory
{
    private int attempt = 0;

    public override Func<Chain> Factory()
    {
        var name = GetType().Name;

        return () => Chain.Create(name)
            .Then(new TaskManagerTask(() =>
            {
                if (IsComplete())
                {
                    return true;
                }

                if (attempt >= GetMaxAttempts())
                {
                    Logger.Debug($"{name}: Max retry attempts reached.");
                    return true;
                }

                var watcher = ChainManager.Get($"{name}##watcher");
                if (!watcher.IsRunning)
                {
                    if (EzThrottler.Throttle(name, GetThrottle()))
                    {
                        attempt++;
                        Logger.Debug($"{name}: Attempt {attempt}/{GetMaxAttempts()}");
                        watcher.Submit(() => Create(Chain.Create(name)));
                    }
                }

                return false;
            }, new() { TimeLimitMS = GetTimeout() }));
    }

    public abstract bool IsComplete();

    public virtual int GetThrottle() => 500;

    public virtual int GetTimeout() => 10000;

    public virtual int GetMaxAttempts() => int.MaxValue;

    public override TaskManagerConfiguration? Config() => new()
    {
        TimeLimitMS = int.MaxValue
    };
}
