using System.Threading;
using System.Threading.Tasks;

namespace Ocelot.Chain;

public class ChainContext
{
    public CancellationTokenSource source { get; }
    public CancellationToken token => source.Token;

    private TaskCompletionSource<bool> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task CompletionTask => completion?.Task ?? Task.CompletedTask;

    public ChainContext(CancellationTokenSource source)
    {
        this.source = source;
    }

    public void Complete() => completion?.TrySetResult(true);
}
