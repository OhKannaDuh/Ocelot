using System.Numerics;
using System.Threading;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
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

    public ProwlerContext(VNavmesh vnav, float distanceThreshold = 5f, float lastMovedThresholod = 0.25f)
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

        var current2D = new Vector2(player.Position.X, player.Position.Z);
        var last2D = new Vector2(last.X, last.Z);

        bool hasMoved = Vector2.Distance(current2D, last2D) > lastMovedThresholod;

        last = player.Position;

        return hasMoved;
    }

    public unsafe void Jump() => ActionManager.Instance()->UseAction(ActionType.GeneralAction, 2);

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

    public bool IsNear(Vector3 a, Vector3 b, float threshold = 1f) => Vector3.DistanceSquared(a, b) <= threshold;

    public bool IsAtDestination(Vector3 destination)
    {
        var player = Svc.ClientState.LocalPlayer;
        if (player == null)
        {
            Logger.Error("[Prowler] Unable to get player");
            return false;
        }


        Logger.Debug($"[Prowler] Distance: {Vector3.Distance(player.Position, destination)} <= {distanceThreshold}");

        return Vector3.Distance(player.Position, destination) <= distanceThreshold;
    }

}
