using Microsoft.Extensions.DependencyInjection;
using Ocelot.Ipc.VNavmesh;
using Ocelot.Services.Pathfinding;

namespace Ocelot.Pathfinding.Services.Navmesh;

public class NavmeshPathfindingProvider(IVNavmeshIpc vnav, IServiceProvider services) : IPathfindingProvider
{
    public const string Key = "vnavmesh";

    public string InternalName
    {
        get => Key;
    }

    public string DisplayName
    {
        get => Key;
    }

    public bool IsAvailable()
    {
        return vnav.IsAvailable();
    }

    public IPathfinder Create()
    {
        return services.GetRequiredService<NavmeshPathfinder>();
    }
}
