using System;
using System.Collections.Generic;
using System.Numerics;
using Ocelot.Chain;
using Ocelot.Prowler;

public class Prowler
{
    public static Func<Chain> Create(ProwlerContext context, List<IProwlerAction> actions)
    {
        return () =>
        {
            var chain = Chain.Create();
            foreach (var action in actions)
            {
                chain.Then(action.Create(context.Clone()));
            }

            return chain;
        };
    }

    public static IProwlerAction Start(Vector3 node, bool fly = false) => Pathfind(node, fly);

    public static IProwlerAction Start(float x, float y, float z, bool fly = false) => Start(new(x, y, z), fly);

    public static IProwlerAction FollowPath(List<Vector3> path, bool fly = false) => new FollowPathAction(path, fly);

    public static IProwlerAction FollowPath(List<float[]> path, bool fly = false)
    {
        List<Vector3> actualPath = [];
        foreach (var point in path)
        {
            if (point.Length == 3)
            {
                actualPath.Add(new Vector3(point[0], point[1], point[2]));
            }
        }

        return FollowPath(actualPath, fly);
    }

    public static IProwlerAction Jump() => new JumpAction();

    public static IProwlerAction MoveTo(Vector3 node, bool fly = false) => new MoveToAction(node, fly);

    public static IProwlerAction MoveTo(float x, float y, float z, bool fly = false) => MoveTo(new(x, y, z), fly);

    public static IProwlerAction Pathfind(Vector3 node, bool fly = false) => new PathfindAction(node, fly);

    public static IProwlerAction Pathfind(float x, float y, float z, bool fly = false) => Pathfind(new(x, y, z), fly);
}
