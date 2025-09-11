using System;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using Ocelot.Services;
using Ocelot.Services.Logger;

namespace Ocelot.Chain.Steps;

public class InteractStep(string name, Func<IGameObject?> getObject, TimeSpan? delay = null, uint maxAttempts = 0) : RetryStep(name)
{
    private static ILoggerService Logger {
        get => OcelotServices.GetCached<ILoggerService>();
    }

    protected override TimeSpan GetDelayForAttempt(uint attempt)
    {
        return delay ?? TimeSpan.FromMilliseconds(500);
    }

    protected override uint MaxAttempts {
        get => maxAttempts == 0 ? 5 : maxAttempts;
    }

    protected override unsafe ValueTask<bool> Action(ChainContext context, CancellationToken token)
    {
        Logger.Debug("Attempting to interact with object {Attempt}/{Max}", Attempt, MaxAttempts);
        var obj = getObject();
        if (obj is null || obj.IsDead || !obj.IsTargetable)
        {
            Logger.Debug("Object was null, dead or untargetable. ({null}, {dead}, {untargetable}", obj is null, obj?.IsDead, obj?.IsTargetable == false);
            return ValueTask.FromResult(false);
        }

        if (!obj.IsTarget())
        {
            Logger.Debug("Object was not targeted.");
            Svc.Targets.Target = obj;
            return ValueTask.FromResult(false);
        }

        // This didn't work for aetherytes
        // if (EventFramework.Instance()->CheckInteractRange(Player.GameObject, obj.Struct(), 1, false))
        // {
        //     Logger.Error("Object is out of interaction range.");
        //     return ValueTask.FromResult(false);
        // }

        Logger.Debug("Attempting Interaction");
        return ValueTask.FromResult(TargetSystem.Instance()->InteractWithObject(obj.Struct(), false) != 0);
    }
}
