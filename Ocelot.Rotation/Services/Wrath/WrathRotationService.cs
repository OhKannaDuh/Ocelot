using Dalamud.Plugin;
using WrathCombo.API;
using WrathCombo.API.Enum;

namespace Ocelot.Rotation.Services.Wrath;

public class WrathRotationService(IDalamudPluginInterface pluginInterface, OcelotPlugin plugin) : IRotationService, IDisposable
{
    private readonly Lock gate = new();

    private Lazy<Guid?> lease = NewLease(pluginInterface, plugin);

    private static Lazy<Guid?> NewLease(IDalamudPluginInterface pluginInterface, OcelotPlugin plugin)
    {
        return new Lazy<Guid?>(
            () =>
            {
                WrathIPCWrapper.Init(pluginInterface, WrathIPCWrapper.ErrorType.All);
                return WrathIPCWrapper.RegisterForLease(plugin.Name, plugin.Name);
            },
            LazyThreadSafetyMode.ExecutionAndPublication);
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
            lease = NewLease(pluginInterface, plugin);
        }

        if (old is { IsValueCreated: true, Value: { } id })
        {
            WrathIPCWrapper.ReleaseControl(id);
        }
    }


    public void EnableAutoRotation()
    {
        if (Lease.HasValue)
        {
            WrathIPCWrapper.SetAutoRotationState(Lease.Value, true);
        }
    }

    public void DisableAutoRotation()
    {
        if (Lease.HasValue)
        {
            WrathIPCWrapper.SetAutoRotationState(Lease.Value, false);
        }
    }


    private int? cachedDPSAoETargets;

    public void EnableSingleTarget()
    {
        if (Lease.HasValue)
        {
            var current = WrathIPCWrapper.GetAutoRotationConfigState(AutoRotationConfigOption.DPSAoETargets);
            if (current != null)
            {
                cachedDPSAoETargets = current as int?;
            }

            WrathIPCWrapper.SetAutoRotationConfigState(Lease.Value, AutoRotationConfigOption.DPSAoETargets, 99);
        }
    }

    public void DisableSingleTarget()
    {
        if (Lease.HasValue && cachedDPSAoETargets.HasValue)
        {
            WrathIPCWrapper.SetAutoRotationConfigState(Lease.Value, AutoRotationConfigOption.DPSAoETargets, cachedDPSAoETargets.Value);
            cachedDPSAoETargets = null;
        }
    }

    public void Dispose()
    {
        Unload();
    }
}
