using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using Ocelot.Chain.Middleware;
using Ocelot.Chain.Steps;
using Ocelot.Internal;
using Ocelot.Services;
using Ocelot.Services.Logger;

namespace Ocelot.Chain;

public sealed class ChainRunner : IDisposable
{
    private readonly Deque<IChainStep> chainSteps = [];

    private readonly List<IStepMiddleware> stepMiddleware = [];

    private readonly CancellationTokenSource cancellationTokenSource = new();

    private readonly ChainContext chainContext;

    private readonly IChainStepResultHandler handler;

    private readonly DateTime createdAt = DateTime.UtcNow;

    private readonly int initialCount;

    public readonly string Name;

    private string previousStepName = "";

    private long stepStartTime = 0;

    private static ILoggerService Logger {
        get => OcelotServices.GetCached<ILoggerService>();
    }

    public TimeSpan TimeAlive {
        get => DateTime.UtcNow - createdAt;
    }

    public int TotalSteps {
        get => initialCount;
    }

    public int StepsCompleted { get; private set; }

    public int StepsRemaining {
        get => chainSteps.Count;
    }

    public float Progress {
        get => TotalSteps == 0 ? 1f : (float)StepsCompleted / TotalSteps;
    }


    public bool IsIdle { get; private set; } = true;

    public bool IsCompleted {
        get => chainSteps.Count == 0 && IsIdle;
    }

    public ChainRunner(string name, IEnumerable<IChainStep> steps, ChainContext context, IEnumerable<IStepMiddleware> middleware, IChainStepResultHandler? handler = null)
    {
        Name = name;
        foreach (var s in steps)
        {
            chainSteps.AddToBack(s);
            initialCount++;
        }

        chainContext = context;
        stepMiddleware = middleware.ToList();
        this.handler = handler ?? new BaseChainStepResultHandler();
    }

    public void EnqueueFront(params IChainStep[] stepsToAdd)
    {
        foreach (var step in stepsToAdd)
        {
            chainSteps.AddToFront(step);
        }
    }

    public void EnqueueBack(params IChainStep[] stepsToAdd)
    {
        foreach (var step in stepsToAdd)
        {
            chainSteps.AddToBack(step);
        }
    }

    public void Start()
    {
        Logger.Debug("Chain {chain} starting with {total} total steps", Name, TotalSteps);
        Svc.Framework.Update += Tick;
    }


    public void Tick(IFramework _)
    {
        Tick();
    }

    public void Tick()
    {
        if (chainSteps.Count == 0)
        {
            IsIdle = true;
            return;
        }

        IsIdle = false;

        var remainingBefore = chainSteps.Count;
        var step = chainSteps.RemoveFromFront();

        if (step.Name != previousStepName)
        {
            if (previousStepName != "")
            {
                var elapsed = (Stopwatch.GetTimestamp() - stepStartTime) * 1000.0 / Stopwatch.Frequency;
                Logger.Debug("Step {name} completed in {elapsed:0.##} ms (chain {chain}, completed {completed}/{total})", step.Name, elapsed, Name, StepsCompleted, TotalSteps);
            }

            Logger.Debug("Starting step {name} (chain {chain}, remaining {remaining}/{total}, progress {progress:P0})", step.Name, Name, remainingBefore, TotalSteps, Progress);
            stepStartTime = Stopwatch.GetTimestamp();
            previousStepName = step.Name;
        }

        var pipeline = BuildPipeline(0);
        var result = pipeline(step, chainContext, cancellationTokenSource.Token).AsTask().GetAwaiter().GetResult();

        handler.HandleResult(step, result, chainSteps);

        if (result.Status is ChainStepStatus.Continue or ChainStepStatus.Done)
        {
            StepsCompleted++;
        }
    }

    private StepDelegate BuildPipeline(int idx)
    {
        return (step, ctx, ct) => {
            if (idx >= stepMiddleware.Count)
            {
                return step.ExecuteAsync(ctx, ct);
            }

            var current = stepMiddleware[idx];
            return current.InvokeAsync(step, ctx, ct, BuildPipeline(idx + 1));
        };
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        Svc.Framework.Update -= Tick;
    }
}
