using System;
using Dalamud.Plugin.Services;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;

namespace Ocelot.Chain;

public class Chain
{
    private TaskManager tasks;

    private ChainContext context = new();

    public readonly string name = "Unnamed";

    public float progress => tasks.Progress;

    private Chain(string name, TaskManagerConfiguration? defaultConfiguration = null)
    {
        this.name = name;

        if (defaultConfiguration == null)
        {
            defaultConfiguration = new()
            {
                TimeLimitMS = int.MaxValue,
            };
        }

        tasks = new(defaultConfiguration);

        Svc.Framework.Update += Tick;

        Log($"Starting Chain [{name}]");
    }

    public static Chain Create(string name, TaskManagerConfiguration? defaultConfiguration = null) => new(name, defaultConfiguration);

    public static Chain Create(TaskManagerConfiguration? defaultConfiguration = null) => Create("Unnamed", defaultConfiguration);

    private void Tick(IFramework _)
    {
        if (context.token.IsCancellationRequested && !IsComplete())
        {
            tasks.Abort();
            return;
        }
    }

    public Chain Then(Action<ChainContext> action)
    {
        tasks.Enqueue(() => action(context));
        return this;
    }

    public Chain ConditionalThen(Func<ChainContext, bool> condition, Action<ChainContext> action)
    {
        if (condition(context))
        {
            Then(action);
        }

        return this;
    }

    public Chain Then(TaskManagerTask task)
    {
        tasks.EnqueueMulti(task);
        return this;
    }

    public Chain ConditionalThen(Func<ChainContext, bool> condition, TaskManagerTask task)
    {
        if (condition(context))
        {
            Then(task);
        }

        return this;
    }

    public Chain Then(Func<Chain> factory, TaskManagerConfiguration? config = null)
    {
        Chain? chain = null;
        return Then(new TaskManagerTask(() =>
        {
            if (chain == null)
            {
                chain = factory();
                Logger.Debug($"Creating chain {chain.name} from factory");
            }

            return chain.IsComplete();
        }, config));
    }

    public Chain ConditionalThen(Func<ChainContext, bool> condition, Func<Chain> factory, TaskManagerConfiguration? config = null)
    {
        if (condition(context))
        {
            Then(factory, config);
        }

        return this;
    }

    public Chain Then(ChainFactory chain) => Then(chain.Factory(), chain.Config());

    public Chain ConditionalThen(Func<ChainContext, bool> condition, ChainFactory chain)
    {
        if (condition(context))
        {
            Then(chain);
        }

        return this;
    }

    public Chain Wait(int delay)
    {
        tasks.EnqueueDelay(delay);
        return this;
    }

    public Chain Info(string message) => Then(_ => Logger.Info(message));

    public Chain Log(string message) => Info(message);

    public Chain Error(string message) => Then(_ => Logger.Error(message));

    public Chain Debug(string message) => Then(_ => Logger.Debug(message));

    public void Abort() => tasks.Abort();

    public bool IsComplete() => tasks.IsBusy == false && tasks.NumQueuedTasks == 0;
}
