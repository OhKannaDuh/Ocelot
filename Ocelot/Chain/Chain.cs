using System;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using ECommons.Throttlers;

namespace Ocelot.Chain;

public class Chain
{
    private TaskManager tasks;

    private ChainContext context = new();

    private Chain(TaskManagerConfiguration? defaultConfiguration = null)
    {
        tasks = new(defaultConfiguration);

        Svc.Framework.Update += Tick;
    }

    public static Chain Create(TaskManagerConfiguration? defaultConfiguration = null) => new(defaultConfiguration);

    public static Chain Create(string name, TaskManagerConfiguration? defaultConfiguration = null) => Create(defaultConfiguration).Log($"Starting Chain {name}");

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

    public Chain Then(TaskManagerTask task)
    {
        tasks.EnqueueMulti(task);
        return this;
    }

    public Chain Then(Chain chain) => Then(new TaskManagerTask(chain.IsComplete));

    public Chain Then(Func<Chain> factory)
    {
        Chain? chain = null;
        return Then(new TaskManagerTask(() =>
        {
            if (chain == null)
            {
                Logger.Info("Creating chain from factory");
                chain = factory();
            }

            return chain.IsComplete();
        }));
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
