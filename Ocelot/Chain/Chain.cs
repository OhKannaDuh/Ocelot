using System;
using Dalamud.Plugin.Services;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;

namespace Ocelot.Chain;

public class Chain
{
    private readonly TaskManager tasks;

    private readonly ChainContext context = new();

    public readonly string Name;

    public float Progress {
        get => tasks.Progress;
    }

    private Chain(string name, TaskManagerConfiguration? defaultConfiguration = null)
    {
        Name = name;

        if (defaultConfiguration == null)
        {
            defaultConfiguration = new TaskManagerConfiguration {
                TimeLimitMS = int.MaxValue,
            };
        }

        tasks = new TaskManager(defaultConfiguration);

        Svc.Framework.Update += Tick;

        Debug($"Starting Chain [{Name}]");
    }

    public static Chain Create(string name, TaskManagerConfiguration? defaultConfiguration = null)
    {
        return new Chain(name, defaultConfiguration);
    }

    public static Chain Create(TaskManagerConfiguration? defaultConfiguration = null)
    {
        return Create("Unnamed", defaultConfiguration);
    }

    private void Tick(IFramework _)
    {
        if (context.token.IsCancellationRequested && !IsComplete())
        {
            tasks.Abort();
        }
    }

    public Chain Then(Action<ChainContext> action)
    {
        tasks.Enqueue(() => action(context));
        return this;
    }

    public Chain ConditionalThen(Func<ChainContext, bool> condition, Action<ChainContext> action)
    {
        tasks.Enqueue(() => {
            if (condition(context))
            {
                tasks.Insert(() => action(context));
            }
        });

        return this;
    }

    public Chain Then(TaskManagerTask task)
    {
        tasks.EnqueueMulti(task);
        return this;
    }

    public Chain Then(Func<ChainContext, bool?> factory)
    {
        tasks.EnqueueMulti(new TaskManagerTask(() => factory(context)));
        return this;
    }

    public Chain ConditionalThen(Func<ChainContext, bool> condition, TaskManagerTask task)
    {
        tasks.Enqueue(() => {
            if (condition(context))
            {
                tasks.InsertMulti(task);
            }
        });

        return this;
    }

    public Chain Then(Func<Chain> factory, TaskManagerConfiguration? config = null)
    {
        Chain? chain = null;
        return Then(new TaskManagerTask(() => {
            if (chain == null)
            {
                chain = factory();
                Logger.Debug($"Creating chain {chain.Name} from factory");
            }

            return chain.IsComplete();
        }, config));
    }

    public Chain ConditionalThen(Func<ChainContext, bool> condition, Func<Chain> factory, TaskManagerConfiguration? config = null)
    {
        tasks.Enqueue(() => {
            if (condition(context))
            {
                var chain = factory();
                tasks.InsertMulti(new TaskManagerTask(() => chain.IsComplete(), config));
            }
        });

        return this;
    }

    public Chain Then(ChainFactory chain)
    {
        return Then(chain.Factory(), chain.Config());
    }

    public Chain ConditionalThen(Func<ChainContext, bool> condition, ChainFactory chain)
    {
        return ConditionalThen(condition, chain.Factory(), chain.Config());
    }

    public Chain Wait(int delay)
    {
        tasks.EnqueueDelay(delay);
        return this;
    }

    public Chain ConditionalWait(Func<ChainContext, bool> condition, int delay)
    {
        tasks.Enqueue(() => {
            if (condition(context))
            {
                tasks.InsertDelay(delay);
            }
        });

        return this;
    }

    public Chain SubChain(string name, Func<Chain, Chain> subChainFactory, TaskManagerConfiguration? config = null)
    {
        return Then(() => subChainFactory(Create(name)), config);
    }

    public Chain SubChain(Func<Chain, Chain> subChainFactory, TaskManagerConfiguration? config = null)
    {
        return Then(() => subChainFactory(Create()), config);
    }

    public Chain Info(string message)
    {
        return Then(_ => Logger.Info(message));
    }

    public Chain Log(string message)
    {
        return Info(message);
    }

    public Chain Error(string message)
    {
        return Then(_ => Logger.Error(message));
    }

    public Chain Debug(string message)
    {
        return Then(_ => Logger.Debug(message));
    }

    public void Abort()
    {
        tasks.Abort();
    }

    public bool IsComplete()
    {
        return tasks is { IsBusy: false, NumQueuedTasks: 0 };
    }
}
