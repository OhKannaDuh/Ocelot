using System;
using System.Collections.Generic;
using System.Threading;
using Ocelot.Chain.Middleware;
using Ocelot.Chain.Steps;

namespace Ocelot.Chain.Builder;

public sealed partial class ChainBuilder(string name)
{
    private readonly string name = name;

    private readonly List<IChainStep> chainSteps = [];

    private readonly List<IStepMiddleware> stepMiddleware = [];

    public static ChainBuilder Default(string name)
    {
        var builder = new ChainBuilder(name);
        builder.Use(new NextTaskMiddleware());
        builder.Use(new TaskHookMiddleware());

        return builder;
    }

    public ChainBuilder Use(IStepMiddleware middleware)
    {
        stepMiddleware.Add(middleware);
        return this;
    }

    public ChainBuilder Then(IChainStep step)
    {
        chainSteps.Add(step);
        return this;
    }

    public ChainBuilder Then(params IChainStep[] steps)
    {
        foreach (var s in steps)
        {
            Then(s);
        }

        return this;
    }

    public ChainBuilder Then(string stepName, Action<ChainContext, CancellationToken> callback)
    {
        return Then(new CallbackStep(stepName, callback));
    }

    public ChainBuilder Then(string stepName, Action callback)
    {
        return Then(new CallbackStep(stepName, (_, _) => callback()));
    }

    public ChainRunner Build(ChainContext? context = null)
    {
        return new ChainRunner(name, chainSteps, context ??= new ChainContext(), stepMiddleware);
    }
}
