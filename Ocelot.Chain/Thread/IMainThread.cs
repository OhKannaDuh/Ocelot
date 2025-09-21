namespace Ocelot.Chain.Thread;

public interface IMainThread
{
    bool IsMainThread { get; }

    Task SwitchAsync(CancellationToken ct = default);

    Task<T> InvokeAsync<T>(Func<Task<T>> func, CancellationToken ct = default);

    Task InvokeAsync(Func<Task> func, CancellationToken ct = default);

    void Post(Action action);
}
