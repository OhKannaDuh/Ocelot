using System.Numerics;

namespace Ocelot.Prowler;

public class MoveToAction : NavAction
{
    private Vector3 node;

    private bool fly;

    public MoveToAction(Vector3 node, bool fly = false)
        : base(node)
    {
        this.node = node;
        this.fly = fly;
    }

    protected override void Init(ProwlerContext context) => context.vnav.FollowPath([node], fly);

    protected override string GetKey() => $"Prowler.MoveToAction({node}, {fly})";
}
