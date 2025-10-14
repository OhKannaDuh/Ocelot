namespace Ocelot.States;

public interface IStateMachine<out TState>
    where TState : struct, Enum
{
    TState State { get; }

    void Update();
}
