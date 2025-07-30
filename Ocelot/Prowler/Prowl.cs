using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using Ocelot.Chain.ChainEx;
using Ocelot.Extensions;
using Ocelot.Gameplay;
using Ocelot.IPC;

namespace Ocelot.Prowler;

public class Prowl(Vector3 destination)
{
    public readonly Vector3 OriginalDestination = destination;

    public Vector3 FinalDestination { get; private set; } = destination;

    public Vector3 Destination {
        get => FinalDestination;
        set => FinalDestination = value;
    }

    public readonly IGameObject? GameObject;

    public readonly Vector3 OriginalStart = Player.Position;

    public Vector3 FinalStart { get; set; } = Player.Position;

    public Vector3 Start {
        get => FinalStart;
        set => FinalStart = value;
    }

    public Func<Prowl, bool> ShouldFly { get; init; } = _ => true;

    public Func<Prowl, bool> ShouldMount { get; init; } = _ => Svc.Condition[ConditionFlag.InCombat];

    public Func<Prowl, bool> ShouldSprint { get; init; } = _ => true;

    public List<Vector3> OriginalNodes { get; private set; } = [];

    public List<Vector3> FinalNodes { get; private set; } = [];


    public List<Vector3> Nodes {
        get => FinalNodes;
        set => FinalNodes = value;
    }

    public Action<Prowl> PreProcessor { get; init; } = _ => { };

    public Action<Prowl> PostProcessor { get; init; } = _ => { };

    public Func<Prowl, bool> Watcher { get; init; } = _ => false;

    public Action<Prowl, VNavmesh> OnComplete { get; init; } = (prowl, vnavmesh) => { };

    public Action<Prowl, VNavmesh> OnCancel { get; init; } = (prowl, vnavmesh) => { };

    private Task<List<Vector3>>? pathfindingTask = null;

    public float EuclideanDistance {
        get => Vector3.Distance(Start, Destination);
    }

    public float OriginalPathLength {
        get => OriginalNodes.Length();
    }

    public float PathLength {
        get => Nodes.Length();
    }

    public Prowl(IGameObject gameObject)
        : this(gameObject.Position)
    {
        GameObject = gameObject;
    }

    public Func<Chain.Chain> GetChain(VNavmesh vnavmesh)
    {
        return () => Chain.Chain.Create($"Prowl({Start:f2}, {Destination:f2})")
            .Then(_ => !vnavmesh.IsRunning())
            .Then(_ => PreProcessor.Invoke(this))
            .Then(_ => pathfindingTask = vnavmesh.Pathfind(Start, Destination, ShouldFly(this)))
            .BreakIf(() => pathfindingTask == null)
            .Then(_ => !vnavmesh.IsRunning() && pathfindingTask!.IsCompleted)
            .BreakIf(() => pathfindingTask!.IsCanceled || pathfindingTask!.IsFaulted)
            .Then(_ => OriginalNodes = Nodes = pathfindingTask!.Result)
            .Then(_ => PostProcessor.Invoke(this))
            .ConditionalThen(_ => ShouldMount(this) && !Player.Mounted, _ => Actions.MountRoulette.Cast())
            .Then(_ => !ShouldMount(this) || Player.Mounted)
            .ConditionalThen(_ => !ShouldFly(this) && ShouldSprint(this) && !Player.Mounted, _ => Actions.Sprint.Cast())
            .Then(_ => vnavmesh.MoveTo(Nodes, ShouldFly(this)))
            .Then(_ => !vnavmesh.IsRunning() || Watcher.Invoke(this))
            .Then(_ => vnavmesh.Stop())
            .Then(_ => OnComplete.Invoke(this, vnavmesh))
            .OnCancel(_ => OnCancel(this, vnavmesh));
    }

    public void Reset()
    {
        Start = OriginalStart;
        Destination = OriginalDestination;
        OriginalNodes.Clear();
        FinalNodes.Clear();
    }
}
