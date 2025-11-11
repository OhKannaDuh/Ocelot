namespace Ocelot.States;

public interface IStateMachine<TState>
    where TState : struct, Enum
{
    TState State { get; }

    IStateHandler<TState> StateHandler { get; }
    
    string GetStateTranslationKey(TState state);

    void Update();

    void Render();
}
