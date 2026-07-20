using System.Numerics;
using Dalamud.Plugin.Services;
using Ocelot.Chain.Extensions;
using Ocelot.Chain.Middleware.Chain;
using Ocelot.Chain.Middleware.Step;
using Ocelot.Chain.Steps;
using Ocelot.Extensions;
using Ocelot.Ipc.VNavmesh;
using Ocelot.Services.Logger;
using Ocelot.Services.Pathfinding;

namespace Ocelot.Chain.Recipes;

public class
    PathfindToChain(
    IChainFactory chains,
    IPathfinder pathfinder,
    IVNavmeshIpc vnav,
    IObjectTable objects,
    ILogger<PathfindToChain> logger
) : ChainRecipe<PathfinderConfig>(chains)
{
    private static readonly TimeSpan NavmeshReadyTimeout = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan MovementStartTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan MovementCompleteTimeout = TimeSpan.FromMinutes(5);

    public override string Name { get; } = "Pathfind to Chain";

    protected override IChain Compose(IChain chain, PathfinderConfig pathfinderConfig)
    {
        return chain
            .UseMiddleware<LogChainMiddleware>()
            .UseMiddleware(new RetryChainMiddleware(logger)
            {
                DelayMs = 500,
                MaxAttempts = 5,
            })
            .UseStepMiddleware<RunOnMainThreadMiddleware>()
            .UseStepMiddleware<LogStepMiddleware>()
            .Then(new WaitUntilStep(_ => new ValueTask<bool>(vnav.IsNavmeshReady()), NavmeshReadyTimeout, name: "Wait for navmesh"))
            .Then(_ =>
            {
                if (IsAtDestination(pathfinderConfig))
                {
                    return new ValueTask<StepResult>(StepResult.Break());
                }

                return new ValueTask<StepResult>(StepResult.Success());
            }, "Distance Check")
            .Then(_ =>
            {
                pathfinder.Stop();
                pathfinder.PathfindAndMoveTo(pathfinderConfig);
                return StepResult.Success();
            }, "Start Pathfinder")
            .Then(new WaitUntilStep(_ => new ValueTask<bool>(IsAtDestination(pathfinderConfig) || pathfinder.GetState() != PathfindingState.Idle),
                MovementStartTimeout,
                name: "Wait for movement start"))
            .Then(new WaitUntilStep(_ => new ValueTask<bool>(IsAtDestination(pathfinderConfig)),
                MovementCompleteTimeout,
                name: "Wait for movement complete"))
            .Then(_ =>
            {
                if (!IsAtDestination(pathfinderConfig))
                {
                    logger.Warning(
                        "Pathfind did not reach destination. Distance={Distance:F2}, State={State}, VnavRunning={Running}, VnavPathfinding={Pathfinding}, NavmeshReady={NavmeshReady}",
                        DistanceToDestination(pathfinderConfig),
                        pathfinder.GetState(),
                        vnav.IsRunning(),
                        vnav.IsPathfinding(),
                        vnav.IsNavmeshReady());

                    return new ValueTask<StepResult>(StepResult.Failure("Did not reach destination"));
                }

                return new ValueTask<StepResult>(StepResult.Success());
            }, "Destination Check");
    }

    private Vector3 PlayerPosition()
    {
        return objects.LocalPlayer?.Position ?? Vector3.NaN;
    }

    private float ArrivalThreshold(PathfinderConfig config)
    {
        return config.DistanceThreshold > 0f ? config.DistanceThreshold : 2f;
    }

    /// <summary>
    ///     Match the point PathfindAndMoveTo actually moves toward (including floor snap).
    /// </summary>
    private Vector3 ResolvedDestination(PathfinderConfig config)
    {
        Vector3 destination = config.To();
        if (config.ShouldSnapToFloor)
        {
            destination = pathfinder.SnapToMesh(destination, config.FloorSnapExtents);
        }

        return destination;
    }

    private float DistanceToDestination(PathfinderConfig config)
    {
        Vector3 destination = ResolvedDestination(config);
        Vector3 player = PlayerPosition();

        // Ground movement: ignore bad destination Y (authored points are often slightly underground).
        if (!config.AllowFlying)
        {
            return player.Distance2D(destination);
        }

        return Vector3.Distance(player, destination);
    }

    private bool IsAtDestination(PathfinderConfig config)
    {
        return DistanceToDestination(config) <= ArrivalThreshold(config);
    }
}
