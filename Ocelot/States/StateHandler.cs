using System;
using Ocelot.Modules;
using Ocelot.Windows;

namespace Ocelot.States;

public abstract class StateHandler<TState, TModule>(TModule module) : IDisposable
    where TState : struct, Enum
    where TModule : IModule
{
    public event Action<TModule>? OnEnter;

    public event Action<TModule, TState>? OnExit;

    protected readonly TModule Module = module;
    
    protected DateTime EnterTime = DateTime.Now;

    public abstract TState? Handle();

    public virtual void Enter()
    {
        EnterTime = DateTime.Now;
        OnEnter?.Invoke(Module);
    }

    public virtual void Exit(TState nextState)
    {
        OnExit?.Invoke(Module, nextState);
    }

    public virtual void Dispose()
    {
        OnEnter = null;
        OnExit = null;
    }

    public virtual void Render(RenderContext context)
    {
    }

    public string T(string key)
    {
        return module.T(key);
    }

    public TimeSpan GetTimeInState()
    {
        return DateTime.Now - EnterTime;
    }
}
