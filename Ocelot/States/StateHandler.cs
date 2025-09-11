using System;
using Ocelot.Windows;

namespace Ocelot.States;

public abstract class StateHandler<TState, TContext> : IDisposable
    where TState : struct, Enum
    where TContext : class?
{
    public event Action? OnEnter;

    public event Action<TState>? OnExit;

    protected DateTime EnterTime = DateTime.Now;

    public abstract TState? Handle(TContext? context = null);

    public virtual void Enter(TContext? prowl = null)
    {
        EnterTime = DateTime.Now;
        OnEnter?.Invoke();
    }

    public virtual void Exit(TState nextState, TContext? context = null)
    {
        OnExit?.Invoke(nextState);
    }

    public virtual void Dispose()
    {
        OnEnter = null;
        OnExit = null;
    }

    public virtual void Render(RenderContext context) { }


    public TimeSpan GetTimeInState()
    {
        return DateTime.Now - EnterTime;
    }
}
