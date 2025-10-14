namespace Ocelot.States.Score;

public abstract class ScoreStateHandler<TState> : IScoreStateHandler<TState>
    where TState : struct, Enum
{
    private DateTime entered = DateTime.Now;

    public abstract TState Handles { get; }

    public abstract double GetScore();

    public abstract void Handle();

    public virtual void Enter()
    {
        entered = DateTime.Now;
    }

    public virtual void Exit(TState next)
    {
    }

    public TimeSpan TimeInState
    {
        get => DateTime.Now - entered;
    }
}
