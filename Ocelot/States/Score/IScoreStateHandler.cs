namespace Ocelot.States.Score;

public interface IScoreStateHandler<TState> : IScoreStateHandler<TState, double>
    where TState : struct, Enum;

public interface IScoreStateHandler<TState, out TScore> : IStateHandler<TState>
    where TState : struct, Enum
    where TScore : IComparable
{
    TScore GetScore();

    void Handle();
}
