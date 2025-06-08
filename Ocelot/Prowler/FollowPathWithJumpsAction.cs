using System.Collections.Generic;
using System.Numerics;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;

namespace Ocelot.Prowler;

public class FollowPathWithJumpsAction : IProwlerAction
{
    private List<Vector3> path;

    private List<Vector3> jumps;

    private Vector3 destination;

    public FollowPathWithJumpsAction(List<Vector3> path, List<Vector3> jumps)
    {
        this.path = path;
        this.jumps = jumps;

        this.destination = path != null && path.Count > 0 ? path[^1] : Vector3.Zero;
    }

    public TaskManagerTask Create(ProwlerContext context)
    {
        return new(() =>
        {
            if (context.ShouldInit())
            {
                context.vnav.FollowPath(path, false);
                return false;
            }

            if (!context.HasStarted() && context.vnav.IsRunning())
            {
                context.Start();
                return false;
            }

            var player = Svc.ClientState.LocalPlayer;
            if (player == null)
            {
                return false;
            }

            if (ShouldJump(player.Position))
            {
                context.Jump();
            }

            return context.Check(Identify(), destination);
        }, new() { TimeLimitMS = path.Count * 150 });
    }

    public string Identify() => $"Prowler.FollowPathWithJumpsAction({path.Count}, {jumps.Count})";

    public bool ShouldJump(Vector3 position)
    {
        foreach (var jumpPoint in jumps)
        {
            if (Vector3.Distance(position, jumpPoint) < 0.5f)
            {
                return true;
            }
        }
        return false;
    }
}
