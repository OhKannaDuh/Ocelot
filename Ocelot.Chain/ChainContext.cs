using System.Collections.Concurrent;

namespace Ocelot.Chain;

public class ChainContext(Guid runId, string name, IServiceProvider serviceProvider, CancellationToken cancellationToken = default) : IChainContext
{
    private readonly ConcurrentDictionary<string, object?> data = new();

    public string ChainName { get; } = name;

    public Guid RunId { get; } = runId;

    public CancellationToken CancellationToken { get; } = cancellationToken;

    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public IDictionary<string, object?> Data
    {
        get => data;
    }

    public ChainContext(string name, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        : this(Guid.NewGuid(), name, serviceProvider, cancellationToken)
    {
    }

    public T? GetData<T>(string key)
    {
        return data.TryGetValue(key, out var value) && value is T typedValue ? typedValue : default;
    }

    public void SetData<T>(string key, T value)
    {
        data[key] = value;
    }
}
