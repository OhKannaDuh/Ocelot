using System.Numerics;
using Ocelot.Ipc;
using Ocelot.Prowler;
using Ocelot.UI;

namespace Ocelot.Services.Pathfinding;

[OcelotService(typeof(ProwlerPathfinderService))]
public class ProwlerPathfinderService : IPathfinderService
{
    public static bool RequirementsMet()
    {
        return VNavmesh.IsLoaded();
    }

    public bool IsValid()
    {
        return RequirementsMet();
    }

    public PathfinderState GetState()
    {
        if (!IsValid())
        {
            return PathfinderState.Unknown;
        }

        var state = Prowler.Prowler.State;
        if (state == null)
        {
            return PathfinderState.Unknown;
        }

        switch (state)
        {
            case ProwlState.Preprocessing:
            case ProwlState.Pathfinding:
            case ProwlState.Postprocessing:
            case ProwlState.PreparingMovement:
            case ProwlState.Mount:
                return PathfinderState.Pathfinding;
            case ProwlState.Moving:
            case ProwlState.Redirecting:
                return PathfinderState.Running;
        }

        return PathfinderState.Idle;
    }

    public void Pathfind(Vector3 from, Vector3 to, PathfindingConfig? config = null)
    {
        Prowler.Prowler.Prowl(new Prowl(new ProwlOptions {
            ArrivalRadius = config?.ArrivalRadius ?? 0.4f,
        }, to));
    }

    public void Stop()
    {
        Prowler.Prowler.Abort();
    }

    public void RenderDebug()
    {
        OcelotUI.LabelledValue("Pathfinder State", GetState());
        OcelotUI.LabelledValue("Prowler State", Prowler.Prowler.State?.ToString() ?? "None");
    }
}
