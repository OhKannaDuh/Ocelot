namespace Ocelot.States.Score;

public abstract class ScoreStateHandler<TState>(TState state) : ScoreStateHandler<TState, double>(state)
    where TState : struct, Enum;

public abstract class ScoreStateHandler<TState, TScore>(TState state) : IScoreStateHandler<TState, TScore>
    where TState : struct, Enum
    where TScore : IComparable
{
    private DateTime entered = DateTime.Now;

    public TState Handles
    {
        get => state;
    }

    public abstract TScore GetScore();

    public abstract void Handle();

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
