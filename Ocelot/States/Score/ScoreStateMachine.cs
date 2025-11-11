using Ocelot.Extensions;
using Ocelot.Services.Translation;
using IUIService = Ocelot.Services.UI.IUIService;

namespace Ocelot.States.Score;

public sealed class ScoreStateMachine<TState, TScore> : IStateMachine<TState>, IDisposable
    where TState : struct, Enum
    where TScore : IComparable
{
    public TState State { get; private set; }

    public IStateHandler<TState> StateHandler
    {
        get => Current;
    }

    private readonly TState initial;

    private readonly ITranslator<ScoreStateMachine<TState, TScore>> translator;

    private readonly IUIService ui;

    private readonly IReadOnlyDictionary<TState, IScoreStateHandler<TState, TScore>> handlers;

    private IScoreStateHandler<TState, TScore> Current
    {
        get => handlers.TryGetValue(State, out var h)
            ? h
            : throw new InvalidOperationException($"No handler for {typeof(TState).Name}.{State}");
    }

    public ScoreStateMachine(
        TState initial,
        ITranslator<ScoreStateMachine<TState, TScore>> translator,
        IUIService ui,
        IEnumerable<IScoreStateHandler<TState, TScore>> handlers
    )
    {
        this.initial = initial;
        State = initial;

        this.translator = translator;
        this.ui = ui;

        var map = new Dictionary<TState, IScoreStateHandler<TState, TScore>>();
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

    public string GetStateTranslationKey(TState state)
    {
        return $"{translator.Scope}.{state.ToString().ToSnakeCase()}";
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

    public void Render()
    {
        ui.LabelledValue(translator.T("generic.state"), translator.T($".{State.ToString().ToSnakeCase()}.label"));
        StateHandler.Render();
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
