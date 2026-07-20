using Dalamud.Plugin.Services;
using Ocelot.Lifecycle;

namespace Ocelot.Chain.Thread;

public sealed class DalamudMainThread(IFramework framework) : IMainThread, IOnStart, IOnStop
{
    public bool IsMainThread => framework.IsInFrameworkUpdateThread;

    public void OnStart()
    {
    }

    public void OnStop()
    {
    }

    public Task SwitchAsync(CancellationToken ct = default)
    {
        if (IsMainThread)
        {
            return Task.CompletedTask;
        }

        return framework.Run(() => { }, ct);
    }

    public Task<T> InvokeAsync<T>(Func<Task<T>> func, CancellationToken ct = default)
    {
        if (IsMainThread)
        {
            return func();
        }

        return framework.RunOnFrameworkThread(func);
    }

    public Task InvokeAsync(Func<Task> func, CancellationToken ct = default)
    {
        if (IsMainThread)
        {
            return func();
        }

        return framework.RunOnFrameworkThread(func);
    }

    public void Post(Action action)
    {
        framework.Run(action);
    }
}
