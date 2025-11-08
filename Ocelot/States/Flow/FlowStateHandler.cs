namespace Ocelot.States.Flow;

public abstract class FlowStateHandler<TState>(TState state) : IFlowStateHandler<TState>
    where TState : struct, Enum
{
    private DateTime entered = DateTime.Now;

    public TState Handles
    {
        get => state;
    }

    public abstract TState? Handle();

    public virtual void Enter()
    {
        entered = DateTime.Now;
    }

    public virtual void Exit(TState next)
    {
    }

    public virtual void Render()
    {
    }

    public TimeSpan TimeInState
    {
        get => DateTime.Now - entered;
    }
}
