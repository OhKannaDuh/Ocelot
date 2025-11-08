namespace Ocelot.States;

public interface IStateMachine<TState>
    where TState : struct, Enum
{
    TState State { get; }

    IStateHandler<TState> StateHandler { get; }

    void Update();
}
