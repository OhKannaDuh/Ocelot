using Ocelot.Ipc.WrathCombo;

namespace Ocelot.Rotation.Services.Wrath;

public sealed class WrathJobRotation(IWrathComboIpc ipc, OcelotPlugin plugin) : IJobRotationBackend, IDisposable
{
    // Costly / not-advised Wrath options — leave off (do not enable or lock).
    private static readonly HashSet<string> OccultOptionsLeftOff = new(StringComparer.Ordinal)
    {
        "Phantom_Chemist_OccultPotion",
        "Phantom_Chemist_OccultEther",
        "Phantom_Chemist_OccultElixir",
        "Phantom_Samurai_Zeninage",
        "Phantom_Geomancer_Suspend",
        "Phantom_Thief_OccultSprint",
    };

    private readonly Lock gate = new();

    private Lazy<Guid?> lease = NewLease(ipc, plugin);

    private bool farmingDefaultsApplied;

    private bool manualTargeting = true;

    private uint? trackedPhantomJobId;

    public JobRotationBackendKind Kind => JobRotationBackendKind.Wrath;

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

    public void Prepare(JobRotationSessionOptions options)
    {
        manualTargeting = options.ManualTargeting;
        if (!Lease.HasValue)
        {
            return;
        }

        EnsureCurrentJobReady();
        ApplyFarmingDefaults();
    }

    public void Enable(CombatActivity activity)
    {
        _ = activity;
        if (!Lease.HasValue)
        {
            return;
        }

        EnsureCurrentJobReady();
        ApplyFarmingDefaults();
        ipc.SetAutoRotationState(Lease.Value, true);
    }

    public void Disable()
    {
        if (Lease.HasValue)
        {
            ipc.SetAutoRotationState(Lease.Value, false);
        }
    }

    public void Refresh()
    {
        if (!Lease.HasValue)
        {
            return;
        }

        EnsureCurrentJobReady();
    }

    public void SyncContentJob(uint? contentJobId)
    {
        if (!Lease.HasValue)
        {
            return;
        }

        lock (gate)
        {
            if (trackedPhantomJobId == contentJobId)
            {
                return;
            }

            trackedPhantomJobId = contentJobId;

            if (contentJobId is { } next)
            {
                TryLockOccultOptimal(Lease.Value, next);
            }
        }
    }

    public void Teardown()
    {
        Disable();
        lock (gate)
        {
            trackedPhantomJobId = null;
        }

        ReleaseLease();
    }

    public void Dispose() => Teardown();

    private void ReleaseLease()
    {
        Lazy<Guid?> old;
        lock (gate)
        {
            old = lease;
            lease = NewLease(ipc, plugin);
            farmingDefaultsApplied = false;
            trackedPhantomJobId = null;
        }

        if (old is { IsValueCreated: true, Value: { } id })
        {
            ipc.ReleaseControl(id);
        }
    }

    private void EnsureCurrentJobReady()
    {
        if (!Lease.HasValue)
        {
            return;
        }

        try
        {
            ipc.SetCurrentJobAutoRotationReady(Lease.Value);
        }
        catch
        {
        }
    }

    private void ApplyFarmingDefaults()
    {
        if (!Lease.HasValue || farmingDefaultsApplied)
        {
            return;
        }

        var id = Lease.Value;
        ipc.SetAutoRotationConfigState(id, WrathAutoRotationConfigOption.InCombatOnly, true);
        ipc.SetAutoRotationConfigState(id, WrathAutoRotationConfigOption.FATEPriority, true);
        ipc.SetAutoRotationConfigState(id, WrathAutoRotationConfigOption.IncludeNPCs, true);
        ipc.SetAutoRotationConfigState(id, WrathAutoRotationConfigOption.OnlyAttackInCombat, true);
        ipc.SetAutoRotationConfigState(id, WrathAutoRotationConfigOption.AutoRez, true);
        ipc.SetAutoRotationConfigState(id, WrathAutoRotationConfigOption.AutoRezDPSJobs, true);
        ipc.SetAutoRotationConfigState(id, WrathAutoRotationConfigOption.DPSRotationMode, manualTargeting ? 0 : 6);
        ipc.SetAutoRotationConfigState(id, WrathAutoRotationConfigOption.HealerRotationMode, manualTargeting ? 0 : 2);

        farmingDefaultsApplied = true;
    }

    private void TryLockOccultOptimal(Guid leaseId, uint phantomJobId)
    {
        try
        {
            string? parent = ipc.GetOccultParentComboName(phantomJobId);
            if (string.IsNullOrEmpty(parent))
            {
                return;
            }

            ipc.SetComboState(leaseId, parent, comboState: true, autoState: true);

            List<string>? options = ipc.GetOccultOptionNames(phantomJobId);
            if (options == null)
            {
                return;
            }

            foreach (string option in options)
            {
                if (OccultOptionsLeftOff.Contains(option))
                {
                    continue;
                }

                ipc.SetComboOptionState(leaseId, option, state: true);
            }
        }
        catch
        {
        }
    }
}
