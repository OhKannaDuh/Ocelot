using Ocelot.Extensions;
using Ocelot.Services.Translation;
using IUIService = Ocelot.Services.UI.IUIService;

namespace Ocelot.States.Flow;

public sealed class FlowStateMachine<TState> : IStateMachine<TState>, IDisposable
    where TState : struct, Enum
{
    public TState State { get; private set; }

    public IStateHandler<TState> StateHandler
    {
        get => Current;
    }

    private readonly TState initial;

    private readonly ITranslator<FlowStateMachine<TState>> translator;

    private readonly IUIService ui;

    private readonly IReadOnlyDictionary<TState, IFlowStateHandler<TState>> handlers;

    private IFlowStateHandler<TState> Current
    {
        get => handlers.TryGetValue(State, out var h)
            ? h
            : throw new InvalidOperationException($"No handler for {typeof(TState).Name}.{State}");
    }

    public FlowStateMachine(
        TState initial,
        ITranslator<FlowStateMachine<TState>> translator,
        IUIService ui,
        IEnumerable<IFlowStateHandler<TState>> handlers
    ) {
        this.initial = initial;
        State = initial;

        this.translator = translator;
        this.ui = ui;

        var map = new Dictionary<TState, IFlowStateHandler<TState>>();
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
        var next = Current.Handle();
        if (next is null || EqualityComparer<TState>.Default.Equals(next.Value, State))
        {
            return;
        }

        Current.Exit(next.Value);
        State = next.Value;
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
