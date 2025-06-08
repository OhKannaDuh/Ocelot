using System.Collections.Generic;
using System.Numerics;

namespace Ocelot.Prowler;

public class FollowPathAction : NavAction
{
    private List<Vector3> path;

    private bool fly;

    public FollowPathAction(List<Vector3> path, bool fly = false)
        : base(path.Count > 0 ? path[^1] : default)
    {
        this.path = path;
        this.fly = fly;
    }

    protected override void Init(ProwlerContext context) => context.vnav.FollowPath(path, fly);

    protected override string GetKey() => $"Prowler.FollowPathAction({path.Count}, {fly})";
}
