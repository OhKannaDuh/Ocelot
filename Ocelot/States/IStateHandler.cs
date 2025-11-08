namespace Ocelot.States;

public interface IStateHandler<TState>
    where TState : struct, Enum
{
    TState Handles { get; }

    void Enter()
    {
    }

    void Exit(TState next)
    {
    }

    TimeSpan TimeInState { get; }

    void Render()
    {
    }
}
