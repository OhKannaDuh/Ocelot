using System.Numerics;
using System.Threading.Tasks;
using Ocelot.IPC;

namespace Ocelot.Prowler;

public static class PathfindingHelper
{
    private static VNavmesh Vnavmesh
    {
        get => OcelotPlugin.Plugin.IPC.GetSubscriber<VNavmesh>();
    }

    public static async Task<PathfindingOption> Calculate(Vector3 start, Vector3 end, bool fly)
    {
        var nodes = await Vnavmesh.Pathfind(start, end, fly);

        return new PathfindingOption(nodes);
    }
}
