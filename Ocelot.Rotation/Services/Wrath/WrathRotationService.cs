using Ocelot.Ipc.WrathCombo;

namespace Ocelot.Rotation.Services.Wrath;

public class WrathRotationService(IWrathComboIpc ipc, OcelotPlugin plugin) : IRotationService, IDisposable
{
    private readonly Lock gate = new();

    private Lazy<Guid?> lease = NewLease(ipc, plugin);

    private static Lazy<Guid?> NewLease(IWrathComboIpc ipc, OcelotPlugin plugin)
    {
        return new Lazy<Guid?>(() => ipc.RegisterForLease(plugin.Name, plugin.Name), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private Guid? Lease
    {
        get
        {
            lock (gate)
            {
                return lease.Value;
            }
        }
    }

    public void Unload()
    {
        Lazy<Guid?> old;
        lock (gate)
        {
            old = lease;
            lease = NewLease(ipc, plugin);
        }

        if (old is { IsValueCreated: true, Value: { } id })
        {
            ipc.ReleaseControl(id);
        }
    }


    public void EnableAutoRotation()
    {
        if (Lease.HasValue)
        {
            ipc.SetAutoRotationState(Lease.Value, true);
        }
    }

    public void DisableAutoRotation()
    {
        if (Lease.HasValue)
        {
            ipc.SetAutoRotationState(Lease.Value, false);
        }
    }


    private int? cachedDPSAoETargets;

    public void EnableSingleTarget()
    {
        if (Lease.HasValue)
        {
            var current = ipc.GetAutoRotationConfigState(WrathAutoRotationConfigOption.DPSAoETargets);
            if (current != null)
            {
                cachedDPSAoETargets = current as int?;
            }

            ipc.SetAutoRotationConfigState(Lease.Value, WrathAutoRotationConfigOption.DPSAoETargets, 99);
        }
    }

    public void DisableSingleTarget()
    {
        if (Lease.HasValue && cachedDPSAoETargets.HasValue)
        {
            ipc.SetAutoRotationConfigState(Lease.Value, WrathAutoRotationConfigOption.DPSAoETargets, cachedDPSAoETargets.Value);
            cachedDPSAoETargets = null;
        }
    }

    public void Dispose()
    {
        Unload();
    }
}
