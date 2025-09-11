using System;
using System.Threading;
using Ocelot.Chain.Steps;

namespace Ocelot.Chain;

public class ChainContext
{
    public CancellationTokenSource Source { get; } = new();

    public CancellationToken Token {
        get => Source.Token;
    }

    private DateTime timeEnteredCurrentTask = DateTime.Now;

    public IChainStep? Current { get; private set; }


    public void SetCurrent(IChainStep step)
    {
        Current = step;
        timeEnteredCurrentTask = DateTime.Now;
    }

    public void NextTask()
    {
        timeEnteredCurrentTask = DateTime.Now;
    }

    public TimeSpan TimeInCurrentTask {
        get => DateTime.Now - timeEnteredCurrentTask;
    }

    public Action<string, TimeSpan>? OnStepCompleted;

    public Action<string, Exception>? OnStepFailed;
}
