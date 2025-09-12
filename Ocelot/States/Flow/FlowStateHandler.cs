using System;

namespace Ocelot.States.Flow;

public abstract class FlowStateHandler<TState> : IFlowStateHandler<TState>
    where TState : struct, Enum
{
    private DateTime entered = DateTime.Now;

    public abstract TState Handles { get; }

    public abstract TState? Handle();

    public virtual void Enter()
    {
        entered = DateTime.Now;
    }

    public virtual void Exit(TState next)
    {
    }

    public TimeSpan TimeInState
    {
        get => DateTime.Now - entered;
    }
}
