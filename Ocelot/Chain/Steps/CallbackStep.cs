using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ocelot.Chain.Steps;

public class CallbackStep(string name, Action<ChainContext, CancellationToken> callback) : ActionStep(name)
{
    protected override ValueTask<bool> Action(ChainContext context, CancellationToken token)
    {
        callback(context, token);
        return ValueTask.FromResult(true);
    }
}
