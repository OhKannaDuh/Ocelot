using System;
using Ocelot.Chain;
using Ocelot.IPC;

namespace Ocelot.Prowler;

public class Prowler
{
    private static Prowler? CachedInstance = null;

    private static Prowler Instance {
        get {
            if (CachedInstance == null)
            {
                throw new InvalidOperationException("Prowler has not been initialized. Call Initialize(plugin) first.");
            }

            return CachedInstance;
        }
    }

    private readonly OcelotPlugin plugin;

    private VNavmesh Vnavmesh {
        get => Instance.plugin.IPC.GetProvider<VNavmesh>();
    }

    private ChainQueue Chain {
        get => ChainManager.Get("Ocelot.Prowler.Prowler.ChainQueue");
    }

    public static bool IsRunning {
        get => Instance.Chain.IsRunning || Instance.Vnavmesh.IsRunning() || Instance.Vnavmesh.IsPathfinding();
    }

    private Prowler(OcelotPlugin plugin)
    {
        this.plugin = plugin;
    }

    public static void Initialize(OcelotPlugin plugin)
    {
        CachedInstance ??= new Prowler(plugin);
    }

    public static void Prowl(Prowl prowl)
    {
        Instance.Chain.Submit(prowl.GetChain(Instance.Vnavmesh));
    }

    public static void Abort()
    {
        Instance.Vnavmesh.Stop();
        Instance.Chain.Abort();
    }
}
