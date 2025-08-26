using System;
using System.Collections.Generic;
using System.Reflection;
using Ocelot.Modules;
using Ocelot.UI;
using Ocelot.Windows;

namespace Ocelot.States;

public class StateMachine<TState, TModule> : IDisposable
    where TState : struct, Enum
    where TModule : IModule
{
    public TState State { get; protected set; }

    private readonly TState initial;

    public readonly Dictionary<TState, StateHandler<TState, TModule>> Handlers = [];

    protected readonly TModule Module;

    protected StateHandler<TState, TModule> CurrentHandler
    {
        get
        {
            if (Handlers.TryGetValue(State, out var handler))
            {
                return handler;
            }

            throw new InvalidOperationException($"No handler found for current state '{State}'");
        }
    }

    public StateMachine(TState state, TModule module)
    {
        State = state;
        initial = State;
        Module = module;

        foreach (var type in Registry.GetTypesForStateMachine<TState, TModule>())
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

            if (Activator.CreateInstance(type, module) is not StateHandler<TState, TModule> instance)
            {
                throw new InvalidOperationException($"Failed to create instance of {type.FullName} with module argument.");
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

        var transition = CurrentHandler.Handle();
        if (transition == null || State.Equals(transition.Value))
        {
            return;
        }

        Logger.Debug($"Updating state from '{State.GetType().Name}.{State}' to '{State.GetType().Name}.{transition.Value}'");

        CurrentHandler.Exit(transition.Value);
        State = transition.Value;
        CurrentHandler.Enter();
    }

    public void SetState(TState state)
    {
        Logger.Debug($"Updating state from '{State.GetType().Name}.{State}' to '{State.GetType().Name}.{state}'");
        State = state;
        CurrentHandler.Enter();
    }

    public void SetStateWithExit(TState state)
    {
        CurrentHandler.Exit(state);
        SetState(state);
    }

    public void Reset()
    {
        State = initial;
        CurrentHandler.Enter();
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

    public string T(string key)
    {
        return Module.T(key);
    }

    public bool TryGetCurrentHandler<THandler>(out THandler handler)
        where THandler : StateHandler<TState, TModule>
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
}
