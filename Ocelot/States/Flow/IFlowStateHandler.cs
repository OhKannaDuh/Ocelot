namespace Ocelot.States.Flow;

public interface IFlowStateHandler<TState>
    where TState : struct, Enum
{
    TState Handles { get; }

    TState? Handle();

    void Enter()
    {
    }

    void Exit(TState next)
    {
    }

    TimeSpan TimeInState { get; }
}
