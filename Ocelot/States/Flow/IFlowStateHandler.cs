namespace Ocelot.States.Flow;

public interface IFlowStateHandler<TState> : IStateHandler<TState>
    where TState : struct, Enum
{
    TState? Handle();
}
