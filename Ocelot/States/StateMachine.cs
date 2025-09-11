using System;
using System.Collections.Generic;
using System.Reflection;
using Ocelot.UI;
using Ocelot.Windows;

namespace Ocelot.States;

public class StateMachine<TState>(TState state) : StateMachine<TState, object>(state)
    where TState : struct, Enum;

public class StateMachine<TState, TContext> : IDisposable
    where TState : struct, Enum
    where TContext : class?
{
    public TState State { get; protected set; }

    private readonly TState initial;

    public readonly Dictionary<TState, StateHandler<TState, TContext>> Handlers = [];

    protected StateHandler<TState, TContext> CurrentHandler {
        get {
            if (Handlers.TryGetValue(State, out var handler))
            {
                return handler;
            }

            throw new InvalidOperationException($"No handler found for current state '{State}'");
        }
    }

    public StateMachine(TState state)
    {
        State = state;
        initial = State;

        foreach (var type in Registry.GetTypesForStateMachine<TState, TContext>())
        {
            var attr = type.GetCustomAttribute<StateAttribute<TState>>();
            if (attr == null)
            {
                continue;
            }

            if (Handlers.ContainsKey(attr.State))
            {
                throw new InvalidOperationException($"Duplicate state handler for state '{attr.State}' in type '{type.FullName}'.");
            }

            if (Activator.CreateInstance(type) is not StateHandler<TState, TContext> instance)
            {
                throw new InvalidOperationException($"Failed to create instance of {type.FullName}.");
            }

            Logger.Debug($"Registering handler for '{State.GetType().Name}.{attr.State}'");

            Handlers[attr.State] = instance;
        }
    }

    public virtual void Update()
    {
        if (!ShouldUpdate())
        {
            return;
        }

        var context = GetContext();
        var transition = CurrentHandler.Handle(context);
        if (transition == null || State.Equals(transition.Value))
        {
            return;
        }

        Logger.Debug($"Updating state from '{State.GetType().Name}.{State}' to '{State.GetType().Name}.{transition.Value}'");

        CurrentHandler.Exit(transition.Value, context);
        State = transition.Value;
        CurrentHandler.Enter(context);
    }

    public void SetState(TState state)
    {
        Logger.Debug($"Updating state from '{State.GetType().Name}.{State}' to '{State.GetType().Name}.{state}'");
        State = state;
        CurrentHandler.Enter(GetContext());
    }

    public void SetStateWithExit(TState state)
    {
        CurrentHandler.Exit(state, GetContext());
        SetState(state);
    }

    public void Reset()
    {
        State = initial;
        CurrentHandler.Enter(GetContext());
    }

    protected virtual bool ShouldUpdate()
    {
        return true;
    }

    public virtual void Render(RenderContext context)
    {
        OcelotUI.LabelledValue(State.GetType().Name, State);
        CurrentHandler.Render(context);
    }

    public bool TryGetCurrentHandler<THandler>(out THandler handler)
        where THandler : StateHandler<TState, TContext>
    {
        if (CurrentHandler is THandler typed)
        {
            handler = typed;
            return true;
        }

        handler = null!;
        return false;
    }

    public TimeSpan GetTimeInCurrentState()
    {
        return CurrentHandler.GetTimeInState();
    }

    public virtual void Dispose()
    {
        foreach (var handler in Handlers.Values)
        {
            if (handler is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        Handlers.Clear();
    }

    protected virtual TContext? GetContext()
    {
        return null;
    }
}
