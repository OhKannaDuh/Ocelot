namespace Ocelot.States.Score;

public abstract class ScoreEntranceState<TState>(TState state) : ScoreStateHandler<TState>(state)
    where TState : struct, Enum
{
    public override double GetScore()
    {
        return double.MinValue;
    }

    public override void Handle()
    {
    }
}
