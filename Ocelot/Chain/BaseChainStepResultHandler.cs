using System;
using System.Collections.Generic;
using Ocelot.Chain.Steps;
using Ocelot.Internal;

namespace Ocelot.Chain;

public class BaseChainStepResultHandler : IChainStepResultHandler
{
    private readonly Dictionary<ChainStepStatus, Action<IChainStep, ChainStepResult, Deque<IChainStep>>> handlers;

    public BaseChainStepResultHandler()
    {
        handlers = new Dictionary<ChainStepStatus, Action<IChainStep, ChainStepResult, Deque<IChainStep>>> {
            { ChainStepStatus.Continue, OnContinue },
            { ChainStepStatus.Requeue, OnRequeue },
            { ChainStepStatus.InsertSteps, OnInsertSteps },
            { ChainStepStatus.Done, OnDone },
            { ChainStepStatus.Abort, OnAbort },
            { ChainStepStatus.Fail, OnFail },
        };
    }

    public void HandleResult(IChainStep step, ChainStepResult result, Deque<IChainStep> queue)
    {
        if (!handlers.TryGetValue(result.Status, out var handler))
        {
            throw new ArgumentException($"Status {result.Status} not handled");
        }

        handler(step, result, queue);
        Always(step, result, queue);
    }

    protected virtual void OnContinue(IChainStep step, ChainStepResult result, Deque<IChainStep> queue)
    {
        DisposeStep(step);
    }

    protected virtual void OnRequeue(IChainStep step, ChainStepResult result, Deque<IChainStep> queue)
    {
        queue.AddToFront(step);
    }

    protected virtual void OnInsertSteps(IChainStep step, ChainStepResult result, Deque<IChainStep> queue)
    {
        if (result.Insert is { Count: > 0 })
        {
            for (var i = result.Insert.Count - 1; i >= 0; i--)
            {
                queue.AddToFront(result.Insert[i]);
            }
        }
    }

    protected virtual void OnDone(IChainStep step, ChainStepResult result, Deque<IChainStep> queue)
    {
        DisposeStep(step);
        queue.Clear();
    }

    protected virtual void OnAbort(IChainStep step, ChainStepResult result, Deque<IChainStep> queue)
    {
        DisposeStep(step);
        queue.Clear();
    }

    protected virtual void OnFail(IChainStep step, ChainStepResult result, Deque<IChainStep> queue)
    {
        DisposeStep(step);
        queue.Clear();

        throw result.Error ?? new Exception("Chain failed without exception.");
    }

    protected virtual void Always(IChainStep step, ChainStepResult result, Deque<IChainStep> queue) { }

    private static void DisposeStep(IChainStep step)
    {
        switch (step)
        {
            case IAsyncDisposable asyncDisposable:
                asyncDisposable.DisposeAsync().AsTask().GetAwaiter().GetResult();
                break;
            case IDisposable disposable:
                disposable.Dispose();
                break;
        }
    }
}
