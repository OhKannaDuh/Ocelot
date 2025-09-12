using System;
using System.Collections.Generic;
using System.Linq;

namespace Ocelot.States.Score;

public sealed class ScoreStateMachine<TState> : IStateMachine<TState>, IDisposable
    where TState : struct, Enum
{
    public TState State { get; private set; }

    private readonly TState initial;

    private readonly IReadOnlyDictionary<TState, IScoreStateHandler<TState>> handlers;

    private IScoreStateHandler<TState> Current
    {
        get => handlers.TryGetValue(State, out var h)
            ? h
            : throw new InvalidOperationException($"No handler for {typeof(TState).Name}.{State}");
    }

    public ScoreStateMachine(TState initial, IEnumerable<IScoreStateHandler<TState>> handlers)
    {
        this.initial = initial;
        State = initial;

        var map = new Dictionary<TState, IScoreStateHandler<TState>>();
        foreach (var h in handlers)
        {
            if (map.TryGetValue(h.Handles, out var existing))
            {
                throw new InvalidOperationException($"Duplicate handler for state {h.Handles}: {existing.GetType().Name} and {h.GetType().Name}");
            }

            map[h.Handles] = h;
        }

        this.handlers = map;

        if (this.handlers.TryGetValue(State, out var start))
        {
            start.Enter();
        }
    }

    public void Update()
    {
        Current.Handle();

        var next = handlers
            .ToDictionary(h => h.Key, h => h.Value.GetScore())
            .OrderByDescending(h => h.Value)
            .First();

        if (EqualityComparer<TState>.Default.Equals(next.Key, State))
        {
            return;
        }

        Current.Exit(next.Key);
        State = next.Key;
        Current.Enter();
    }

    private void SetState(TState state)
    {
        if (!handlers.ContainsKey(state))
        {
            throw new InvalidOperationException($"No handler registered for {state}");
        }

        Current.Exit(state);
        State = state;
        Current.Enter();
    }

    public void Reset()
    {
        SetState(initial);
    }

    public TimeSpan GetTimeInCurrentState()
    {
        return Current.TimeInState;
    }

    public void Dispose()
    {
    }
}
