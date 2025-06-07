using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;

namespace Ocelot.Chain;

public class Chain : List<IChainlink>, IChainlink
{
    private int currentIndex = 0;
    private ChainContext context;
    private Task? currentTask;
    private bool isRunning = false;

    public static readonly List<Chain> ActiveChains = new();

    private readonly List<Action<Exception>> onErrorHandlers = new();

    private readonly List<Action> onStopHandlers = new();

    private readonly List<Action> onFinallyHandlers = new();

    public async Task RunAsync(ChainContext context)
    {
        this.context = context;
        currentIndex = 0;
        isRunning = true;
        currentTask = null;

        if (ActiveChains.Count == 0)
        {
            Svc.Framework.Update += Tick;
        }

        ActiveChains.Add(this);

        await context.CompletionTask;
    }
    private void Tick(IFramework framework)
    {
        if (!isRunning || context.token.IsCancellationRequested)
        {
            Stop(stoppedEarly: true);
            return;
        }

        if (currentTask?.IsCompleted == false) return;

        if (currentIndex >= this.Count)
        {
            Stop(stoppedEarly: false);
            return;
        }

        var link = this[currentIndex];

        try
        {
            Logger.Debug($"Running link {currentIndex}/{this.Count}");
            currentTask = link.RunAsync(context);
            Logger.Debug($"Started running link {currentIndex}");
        }
        catch (Exception ex)
        {
            InvokeOnError(ex);
            Stop(stoppedEarly: true);
            return;
        }

        currentIndex++;
    }

    private void Stop(bool stoppedEarly)
    {
        if (!isRunning) return;

        isRunning = false;
        ActiveChains.Remove(this);

        if (ActiveChains.Count == 0)
        {
            Svc.Framework.Update -= Tick;
        }

        if (stoppedEarly)
        {
            InvokeOnStop();
        }

        InvokeOnFinally();
        context.Complete();
    }

    private void InvokeOnError(Exception ex)
    {
        foreach (var handler in onErrorHandlers)
        {
            try
            {
                handler(ex);
            }
            catch (Exception ex2)
            {
                Logger.Error(ex2, "Exception thrown in Chainbuild::OnError");
            }
        }
    }

    private void InvokeOnStop()
    {
        foreach (var handler in onStopHandlers)
        {
            try
            {
                handler();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception thrown in Chainbuild::OnStop");
            }
        }
    }

    private void InvokeOnFinally()
    {
        foreach (var handler in onFinallyHandlers)
        {
            try
            {
                handler();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception thrown in Chainbuild::OnFinally");
            }
        }
    }

    public Chain OnError(Action<Exception> handler)
    {
        onErrorHandlers.Add(handler);
        return this;
    }

    public Chain OnStop(Action handler)
    {
        onStopHandlers.Add(handler);
        return this;
    }

    public Chain OnFinally(Action handler)
    {
        onFinallyHandlers.Add(handler);

        return this;
    }

    public async Task Run() => await ChainRunner.Run(this);
}
