using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using Ocelot.Chain.Builder;
using Ocelot.Chain.Steps;

namespace Ocelot.Chain;

public sealed class ChainQueue : IDisposable
{
    private readonly LinkedList<Func<ChainRunner>> queue = new();

    private ChainRunner? current;

    private readonly DateTime createdAt = DateTime.UtcNow;

    public bool HasRun { get; private set; }

    public int ChainsCompleted { get; private set; }

    public TimeSpan TimeAlive {
        get => DateTime.UtcNow - createdAt;
    }

    public int QueueCount {
        get {
            lock (queue)
            {
                return queue.Count;
            }
        }
    }

    public bool IsRunning {
        get => current is { IsCompleted: false };
    }

    public ChainRunner? Current {
        get => current;
    }

    public void Submit(Func<ChainRunner> factory)
    {
        HasRun = true;
        lock (queue)
        {
            queue.AddLast(factory);
        }
    }

    public void SubmitFront(Func<ChainRunner> factory)
    {
        HasRun = true;
        lock (queue)
        {
            queue.AddFirst(factory);
        }
    }

    public void Submit(ChainBuilder builder)
    {
        Submit(() => builder.Build());
    }

    public void SubmitFront(ChainBuilder builder)
    {
        SubmitFront(() => builder.Build());
    }

    public void Submit(string name, params IChainStep[] steps)
    {
        Submit(() => ChainBuilder.Default(name).Then(steps).Build());
    }

    public void SubmitFront(string name, params IChainStep[] steps)
    {
        SubmitFront(() => ChainBuilder.Default(name).Then(steps).Build());
    }

    public void Abort()
    {
        lock (queue)
        {
            current?.Dispose();
            current = null;
            queue.Clear();
        }
    }

    public void Clear()
    {
        lock (queue)
        {
            queue.Clear();
        }
    }

    public void Tick(IFramework framework)
    {
        if (current is { IsCompleted: false })
        {
            current.Tick(framework);
            return;
        }

        if (current is { IsCompleted: true })
        {
            current.Dispose();
            current = null;
            ChainsCompleted++;
        }

        Func<ChainRunner>? factory = null;
        lock (queue)
        {
            if (queue.Count > 0)
            {
                factory = queue.First!.Value;
                queue.RemoveFirst();
            }
        }

        if (factory is not null)
        {
            current = factory();
            current.Tick(framework);
        }
    }

    public void Dispose()
    {
        current?.Dispose();
        current = null;
        Clear();
    }
}
