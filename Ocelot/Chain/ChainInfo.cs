using System;

namespace Ocelot.Chain;

public sealed class ChainInfo
{
    public required string Name { get; init; }

    public TimeSpan TimeAlive { get; init; }

    public float Progress { get; init; }

    public int TotalSteps { get; init; }

    public int StepsCompleted { get; init; }

    public int StepsRemaining { get; init; }

    public static ChainInfo FromRunner(ChainRunner runner)
    {
        return new ChainInfo {
            Name = runner.Name,
            TimeAlive = runner.TimeAlive,
            Progress = runner.Progress,
            TotalSteps = runner.TotalSteps,
            StepsCompleted = runner.StepsCompleted,
            StepsRemaining = runner.StepsRemaining,
        };
    }
}
