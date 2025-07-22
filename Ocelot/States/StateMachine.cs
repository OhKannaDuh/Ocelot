using System;
using System.Collections.Generic;
using System.Reflection;
using Ocelot.Modules;

namespace Ocelot.States;

public class StateMachine<T, M>
    where T : struct, Enum
    where M : IModule
{
    public T State { get; private set; }

    private readonly T Initial;

    public readonly Dictionary<T, StateHandler<T, M>> Handlers = [];

    private StateHandler<T, M> CurrentHandler
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

    public StateMachine(T state, M module)
    {
        State = state;
        Initial = State;

        foreach (var type in Registry.GetTypesForStateMachine<T, M>())
        {
            var attr = type.GetCustomAttribute<StateAttribute<T>>();
            if (attr == null)
            {
                continue;
            }

            if (Handlers.ContainsKey(attr.State))
            {
                throw new InvalidOperationException($"Duplicate state handler for state '{attr.State}' in type '{type.FullName}'.");
            }

            if (Activator.CreateInstance(type) is not StateHandler<T, M> instance)
            {
                throw new InvalidOperationException($"Failed to create instance of {type.FullName}");
            }

            Logger.Debug($"Registering handler for '{State.GetType().Name}.{attr.State}'");

            Handlers[attr.State] = instance;
        }

        CurrentHandler.Enter(module);
    }

    public void Update(M module)
    {
        if (!ShouldUpdate(module))
        {
            return;
        }

        var transition = CurrentHandler.Handle(module);
        if (transition == null || State.Equals(transition.Value))
        {
            return;
        }

        Logger.Debug($"Updating state from '{State.GetType().Name}.{State}' to '{State.GetType().Name}.{transition.Value}'");

        CurrentHandler.Exit(module);
        State = transition.Value;
        CurrentHandler.Enter(module);
    }

    public void Reset(M module)
    {
        State = Initial;
        CurrentHandler.Enter(module);
    }

    protected virtual bool ShouldUpdate(M module)
    {
        return true;
    }
}
