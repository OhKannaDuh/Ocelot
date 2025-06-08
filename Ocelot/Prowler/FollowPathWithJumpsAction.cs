using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;

namespace Ocelot.Prowler;

public class FollowPathWithJumpsAction : IProwlerAction
{
    private enum State
    {
        NotStarted,
        MovingToStart,
        FollowingPath,
    }

    private List<Vector3> path;

    private List<Vector3> jumps;

    private Vector3 destination;

    public FollowPathWithJumpsAction(List<Vector3> path, List<Vector3> jumps)
    {
        var player = Svc.ClientState.LocalPlayer;
        if (player != null && path != null && path.Count > 0)
        {
            int nearestIndex = FindNearestIndex(path, player.Position);
            if (nearestIndex > 0 && nearestIndex < path.Count)
            {
                path = path.GetRange(nearestIndex, path.Count - nearestIndex);
            }
        }

        this.path = path;
        this.jumps = jumps;

        this.destination = path != null && path.Count > 0 ? path[^1] : Vector3.Zero;
    }

    private int FindNearestIndex(List<Vector3> path, Vector3 playerPos)
    {
        float minDistance = float.MaxValue;
        int nearestIndex = 0;

        for (int i = 0; i < path.Count; i++)
        {
            float distance = Vector3.Distance(playerPos, path[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    public TaskManagerTask Create(ProwlerContext context)
    {
        State state = State.NotStarted;

        return new(() =>
        {
            var player = Svc.ClientState.LocalPlayer;
            if (player == null || path == null || path.Count == 0)
            {
                return false;
            }

            if (context.ShouldInit())
            {
                if (context.IsNear(player.Position, path[0]))
                {
                    state = State.FollowingPath;
                    context.vnav.FollowPath(path, false);
                }
                else
                {
                    state = State.MovingToStart;
                    context.vnav.PathfindAndMoveTo(path[0], false);
                }

                return false;
            }

            if (state == State.MovingToStart)
            {
                if (context.IsNear(player.Position, path[0]))
                {
                    state = State.FollowingPath;
                    context.vnav.FollowPath(path, false);
                }

                return false;
            }

            if (state == State.FollowingPath)
            {
                if (!context.HasStarted() && context.vnav.IsRunning())
                {
                    context.Start();
                    return false;
                }

                if (ShouldJump(player.Position))
                {
                    context.Jump();
                }

                return context.Check(Identify(), destination);
            }

            return false;
        },
        new() { TimeLimitMS = int.MaxValue });
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
