using System.Numerics;
using System.Threading;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using Ocelot.IPC;

namespace Ocelot.Prowler;

public class ProwlerContext
{
    public CancellationTokenSource source { get; } = new();

    public CancellationToken token => source.Token;

    public readonly VNavmesh vnav;

    public readonly float distanceThreshold;

    private readonly float lastMovedThresholod;

    private Vector3 last = Vector3.Zero;

    private bool init = false;

    private bool started = false;

    private int? elapsed = null;

    public ProwlerContext(VNavmesh vnav, float distanceThreshold = 0.0005f, float lastMovedThresholod = 0.25f)
    {
        this.vnav = vnav;
        this.distanceThreshold = distanceThreshold;
        this.lastMovedThresholod = lastMovedThresholod;
    }

    public ProwlerContext Clone() => new ProwlerContext(vnav, distanceThreshold, lastMovedThresholod);

    public bool HasMoved()
    {
        var player = Svc.ClientState.LocalPlayer;
        if (player == null) return false;
        bool hasMoved = Vector3.Distance(player.Position, last) > lastMovedThresholod;
        last = player.Position;

        return hasMoved;
    }

    public unsafe void Jump()
    {
        ActionManager.Instance()->UseAction(ActionType.GeneralAction, 2);
    }

    public bool Check(string key, Vector3 destination, int interval = 100)
    {
        if (elapsed == null)
        {
            Logger.Info("[Prowler] Initialising timer");
            elapsed = -interval;
        }

        if (EzThrottler.Throttle(key, interval))
        {
            elapsed += interval;

            var player = Svc.ClientState.LocalPlayer;
            if (player == null)
            {
                Logger.Error("[Prowler] Unable to get player");
                return false;
            }

            if (!HasMoved() && HasWarmedUp())
            {
                Logger.Info("[Prowler] No significant movement detected, jumping");
                Jump();
            }

            var result = !vnav.IsRunning() || Vector3.Distance(player.Position, destination) <= distanceThreshold;
            if (result)
            {
                Logger.Info("[Prowler] Finished");
            }

            return result;
        }

        return false;
    }

    public bool ShouldInit()
    {
        if (!init)
        {
            Logger.Info("[Prowler] Init");
            init = true;
            return true;
        }

        return false;
    }

    public bool HasStarted() => started;

    public void Start()
    {
        Logger.Info("[Prowler] Starting");
        started = true;
    }

    private bool HasWarmedUp() => elapsed >= 1000;

}
