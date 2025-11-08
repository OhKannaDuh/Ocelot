using System.Numerics;
using Ocelot.Chain.Extensions;
using Ocelot.Chain.Middleware.Chain;
using Ocelot.Chain.Middleware.Step;
using Ocelot.Chain.Steps;
using Ocelot.Services.Logger;
using Ocelot.Services.Pathfinding;
using Ocelot.Services.PlayerState;
using Player = ECommons.GameHelpers.Player;

namespace Ocelot.Chain.Recipes;

public class PathfindToChain(
    IChainFactory chains,
    IPathfinder pathfinder,
    IPlayer player,
    ILogger logger
) : ChainRecipe<PathfinderConfig>(chains)
{
    public override string Name { get; } = "Pathfind to Chain";

    protected override IChain Compose(IChain chain, PathfinderConfig pathfinderConfig)
    {
        var position = Player.Position;

        return chain
            .UseMiddleware<LogChainMiddleware>()
            .UseMiddleware(new RetryChainMiddleware(logger)
            {
                DelayMs = 500,
                MaxAttempts = 5,
            })
            .UseStepMiddleware<LogStepMiddleware>()
            .UseStepMiddleware<RunOnMainThreadMiddleware>()
            .Then(_ =>
            {
                if (Vector3.Distance(player.GetPosition(), pathfinderConfig.To()) < pathfinderConfig.DistanceThreshold)
                {
                    return new ValueTask<StepResult>(StepResult.Break());
                }

                return new ValueTask<StepResult>(StepResult.Success());
            }, "Distance Check")
            .Then(_ => pathfinder.PathfindAndMoveTo(pathfinderConfig), "Start Pathfinder")
            .Then(new WaitUntilStep(_ => new ValueTask<bool>(pathfinder.GetState() != PathfindingState.Idle), TimeSpan.MaxValue))
            .Then(new WaitUntilStep(_ => new ValueTask<bool>(pathfinder.GetState() == PathfindingState.Idle), TimeSpan.MaxValue))
            .Then(_ =>
            {
                if (Vector3.Distance(player.GetPosition(), pathfinderConfig.To()) > pathfinderConfig.DistanceThreshold)
                {
                    return new ValueTask<StepResult>(StepResult.Failure("Did not reach destination"));
                }

                return new ValueTask<StepResult>(StepResult.Success());
            }, "Destination Check");
    }
}
