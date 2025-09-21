using System.Collections.Concurrent;
using Dalamud.Plugin.Services;
using Ocelot.Lifecycle;
using Ocelot.Services.Logger;

namespace Ocelot.Chain.Thread;

public sealed class DalamudMainThread(IFramework framework, ILogger logger) : IMainThread, IOnStart, IOnStop
{
    private readonly ConcurrentQueue<Action> queue = new();

    private SynchronizationContext? previousContext;

    private DalamudSyncContext? context;

    private int mainThreadId;

    private volatile bool running;

    public bool IsMainThread
    {
        get => Environment.CurrentManagedThreadId == mainThreadId;
    }

    public void OnStart()
    {
        mainThreadId = Environment.CurrentManagedThreadId;
        previousContext = SynchronizationContext.Current;
        context = new DalamudSyncContext(queue);
        SynchronizationContext.SetSynchronizationContext(context);
        framework.Update += OnFrameworkUpdate;
        running = true;
    }

    public void OnStop()
    {
        running = false;
        framework.Update -= OnFrameworkUpdate;

        if (SynchronizationContext.Current == context)
        {
            SynchronizationContext.SetSynchronizationContext(previousContext);
        }

        context = null;
        previousContext = null;

        while (queue.TryDequeue(out var work))
        {
            SafeRun(work);
        }
    }

    private void OnFrameworkUpdate(IFramework _)
    {
        if (!running)
        {
            return;
        }

        while (queue.TryDequeue(out var work))
        {
            SafeRun(work);
        }
    }

    private void SafeRun(Action a)
    {
        try
        {
            a();
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
        }
    }

    public Task SwitchAsync(CancellationToken ct = default)
    {
        if (IsMainThread)
        {
            return Task.CompletedTask;
        }

        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var reg = ct.Register(() => tcs.TrySetCanceled(ct));
        Post(() => tcs.TrySetResult());
        return tcs.Task;
    }

    public Task<T> InvokeAsync<T>(Func<Task<T>> func, CancellationToken ct = default)
    {
        if (IsMainThread)
        {
            return func();
        }

        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var reg = ct.Register(() => tcs.TrySetCanceled(ct));
        Post(async () =>
        {
            try
            {
                tcs.TrySetResult(await func());
            }
            catch (OperationCanceledException oce)
            {
                tcs.TrySetCanceled(oce.CancellationToken);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });
        return tcs.Task;
    }

    public Task InvokeAsync(Func<Task> func, CancellationToken ct = default)
    {
        return InvokeAsync(async () =>
        {
            await func();
            return 0;
        }, ct);
    }

    public void Post(Action action)
    {
        if (context is null)
        {
            throw new InvalidOperationException("Main thread not initialized.");
        }

        queue.Enqueue(action);
    }
}
