using System;
using Ocelot.Modules;

namespace Ocelot.States;

public abstract class StateHandler<T, M>
    where T : struct, Enum
    where M : IModule
{
    public event Action<M>? OnEnter;

    public event Action<M>? OnExit;


    public abstract T? Handle(M module);

    public virtual void Enter(M module)
    {
        OnEnter?.Invoke(module);
    }

    public virtual void Exit(M module)
    {
        OnExit?.Invoke(module);
    }
}
