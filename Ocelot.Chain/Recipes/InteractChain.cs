using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using Ocelot.Chain.Extensions;
using Ocelot.Chain.Middleware.Chain;
using Ocelot.Chain.Middleware.Step;
using Ocelot.Chain.Steps;
using Ocelot.Services.Logger;

namespace Ocelot.Chain.Recipes;

public class InteractChain(IChainFactory chains, ITargetManager targets, ILogger logger) : ChainRecipe<Func<IGameObject?>>(chains)
{
    public override string Name { get; } = "Interact Chain";

    protected override IChain Compose(IChain chain, Func<IGameObject?> path)
    {
        return chain
            .UseMiddleware<LogChainMiddleware>()
            .UseMiddleware(new RetryChainMiddleware(logger)
            {
                DelayMs = 500,
                MaxAttempts = 5,
            })
            .UseStepMiddleware<LogStepMiddleware>()
            .UseStepMiddleware<RunOnMainThreadMiddleware>()
            .Then(() =>
            {
                IGameObject? obj;
                try
                {
                    obj = path();
                }
                catch (Exception ex)
                {
                    return StepResult.Failure(ex);
                }
                
                if (obj is null)
                {
                    return StepResult.Failure("Cannot target object, null target provided.");
                }

                if (obj.IsDead)
                {
                    return StepResult.Failure("Cannot target object, it is dead.");
                }

                if (!obj.IsTargetable)
                {
                    return StepResult.Failure("Cannot target object, it is not targetable.");
                }

                if (!obj.IsTarget())
                {
                    logger.Debug("Targeting {obj}", obj.Name);
                    targets.Target = obj;
                }

                unsafe
                {
                    if (TargetSystem.Instance()->InteractWithObject(obj.Struct(), false) != 0)
                    {
                        return StepResult.Success();
                    }
                }

                return StepResult.Failure($"Cannot interact with {obj.Name}.");
            }, "Interact with target")
            .Then(new WaitUntilStep(_ => new ValueTask<bool>(IsInteracting()), TimeSpan.MaxValue))
            .Then(new WaitUntilStep(_ => new ValueTask<bool>(!IsInteracting()), TimeSpan.MaxValue));
    }

    private bool IsInteracting()
    {
        return Svc.Condition.Any([ConditionFlag.OccupiedInEvent, ConditionFlag.OccupiedInQuestEvent, ConditionFlag.OccupiedInCutSceneEvent]);
    }
}
