using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameHelpers;
using Ocelot.Chain.ChainEx;
using Ocelot.IPC;

namespace Ocelot.Prowler;

public class Prowl(Vector3 destination)
{
    public readonly Vector3 OriginalDestination = destination;

    public Vector3 Destination { get; set; } = destination;

    public IGameObject? GameObject;

    public readonly Vector3 OriginalStart = Player.Position;

    public Vector3 Start { get; set; } = Player.Position;

    public bool ShouldFly { get; init; } = true;

    public List<Vector3> OriginalNodes { get; private set; } = [];

    public List<Vector3> Nodes { get; private set; } = [];

    public Func<Prowl, (Vector3 Start, Vector3 End)> PreProcessor { get; init; } = prowl => (prowl.OriginalStart, prowl.OriginalDestination);

    public Func<Prowl, List<Vector3>> PostProcessor { get; init; } = prowl => prowl.OriginalNodes;

    public Func<Prowl, bool> Watcher { get; init; } = _ => false;

    public Action<Prowl, VNavmesh> OnComplete{ get; init; } = (prowl, vnavmesh) => { };

    private Task<List<Vector3>>? pathfindingTask = null;

    public Prowl(IGameObject gameObject)
        : this(gameObject.Position)
    {
        GameObject = gameObject;
    }

    public Func<Chain.Chain> GetChain(VNavmesh vnavmesh)
    {
        return () => Chain.Chain.Create($"Prowl({Start:f2}, {Destination:f2}, {ShouldFly})")
            .Then(_ => !vnavmesh.IsRunning())
            .Then(_ => (Start, Destination) = PreProcessor.Invoke(this))
            .Then(_ => pathfindingTask = vnavmesh.Pathfind(Start, Destination, ShouldFly))
            .BreakIf(() => pathfindingTask == null)
            .Then(_ => !vnavmesh.IsRunning() && pathfindingTask!.IsCompleted)
            .BreakIf(() => pathfindingTask!.IsCanceled || pathfindingTask!.IsFaulted)
            .Then(_ => OriginalNodes = pathfindingTask!.Result)
            .Then(_ => Nodes = PostProcessor.Invoke(this))
            .Then(_ => vnavmesh.MoveTo(Nodes, ShouldFly))
            .Then(_ => !vnavmesh.IsRunning() || Watcher.Invoke(this))
            .Then(_ => vnavmesh.Stop())
            .Then(_ => OnComplete.Invoke(this, vnavmesh));
    }
}
