using System;
using System.Threading;
using Ocelot.Chain.Steps;

namespace Ocelot.Chain.Builder;

public sealed partial class ChainBuilder
{
    public ChainBuilder WaitUntil(string stepName, Func<ChainContext, CancellationToken, bool> callback)
    {
        return Then(new WaitUntilStep(stepName, callback));
    }

    public ChainBuilder WaitUntil(string stepName, Func<bool> callback)
    {
        return Then(new WaitUntilStep(stepName, (_, _) => callback()));
    }
}
