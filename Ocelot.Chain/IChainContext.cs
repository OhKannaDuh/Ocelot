namespace Ocelot.Chain;

public interface IChainContext
{
    string ChainName { get; }

    Guid RunId { get; }

    CancellationToken CancellationToken { get; }

    IServiceProvider ServiceProvider { get; }

    IDictionary<string, object?> Data { get; }

    T? GetData<T>(string key);

    void SetData<T>(string key, T value);
}
