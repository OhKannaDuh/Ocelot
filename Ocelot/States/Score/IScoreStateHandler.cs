using System;

namespace Ocelot.States.Score;

public interface IScoreStateHandler<TState>
    where TState : struct, Enum
{
    double GetScore();

    TState Handles { get; }

    void Handle();

    void Enter()
    {
    }

    void Exit(TState next)
    {
    }

    TimeSpan TimeInState { get; }
}
