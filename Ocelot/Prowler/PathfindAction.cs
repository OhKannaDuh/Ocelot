using System.Numerics;

namespace Ocelot.Prowler;

public class PathfindAction : NavAction
{
    private Vector3 node;

    private bool fly;

    public PathfindAction(Vector3 node, bool fly = false)
        : base(node)
    {
        this.node = node;
        this.fly = fly;
    }

    protected override void Init(ProwlerContext context) => context.vnav.PathfindAndMoveTo(node, fly);

    protected override string GetKey() => $"Prowler.PathfindAction({node}, {fly})";
}
