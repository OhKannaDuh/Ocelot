using System;
using Ocelot.Chain;
using Ocelot.IPC;

namespace Ocelot.Prowler;

public class Prowler
{
    private static Prowler? _instance = null;

    private static Prowler Instance {
        get {
            if (_instance == null)
            {
                throw new InvalidOperationException("Prowler has not been initialized. Call Initialize(plugin) first.");
            }

            return _instance;
        }
    }

    private OcelotPlugin Plugin;

    private VNavmesh Vnavmesh {
        get => Instance.Plugin.IPC.GetProvider<VNavmesh>();
    }

    private ChainQueue Chain {
        get => ChainManager.Get("Ocelot.Prowler.Prowler.ChainQueue");
    }

    public static bool IsRunning {
        get => Instance.Chain.IsRunning || Instance.Vnavmesh.IsRunning() || Instance.Vnavmesh.IsPathfinding();
    }

    private Prowler(OcelotPlugin plugin)
    {
        Plugin = plugin;
    }

    public static void Initialize(OcelotPlugin plugin)
    {
        _instance ??= new Prowler(plugin);
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
