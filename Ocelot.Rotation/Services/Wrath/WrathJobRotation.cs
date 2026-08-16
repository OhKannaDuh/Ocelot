using Dalamud.Plugin;
using Ocelot.Ipc.WrathCombo;
using WrathCombo.API;
using WrathCombo.API.Enum;

namespace Ocelot.Rotation.Services.Wrath;

public sealed class WrathJobRotation(
    IDalamudPluginInterface pluginInterface,
    OcelotPlugin plugin,
    IWrathComboIpc occult) : IJobRotationBackend, IDisposable
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

    private Lazy<Guid?> lease = NewLease(pluginInterface, plugin);

    private bool farmingDefaultsApplied;

    private bool manualTargeting = true;

    private uint? trackedPhantomJobId;

    public JobRotationBackendKind Kind => JobRotationBackendKind.Wrath;

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

    public void Prepare(JobRotationSessionOptions options)
    {
        manualTargeting = options.ManualTargeting;
        if (!Lease.HasValue)
        {
            return;
        }

        // Wrath throws if configs are set before AutoRotationControlled[0] exists.
        WrathIPCWrapper.SetAutoRotationState(Lease.Value, false);
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

        WrathIPCWrapper.SetAutoRotationState(Lease.Value, true);
        EnsureCurrentJobReady();
        ApplyFarmingDefaults();
    }

    public void Disable()
    {
        if (Lease.HasValue)
        {
            WrathIPCWrapper.SetAutoRotationState(Lease.Value, false);
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
            lease = NewLease(pluginInterface, plugin);
            farmingDefaultsApplied = false;
            trackedPhantomJobId = null;
        }

        if (old is { IsValueCreated: true, Value: { } id })
        {
            WrathIPCWrapper.ReleaseControl(id);
        }
    }

    private void EnsureCurrentJobReady()
    {
        if (!Lease.HasValue)
        {
            return;
        }

        WrathIPCWrapper.SetCurrentJobAutoRotationReady(Lease.Value);
    }

    private void ApplyFarmingDefaults()
    {
        if (!Lease.HasValue || farmingDefaultsApplied)
        {
            return;
        }

        var id = Lease.Value;

        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.InCombatOnly, true);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.BypassQuest, false);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.BypassFATE, true);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.UnTargetAndDisableForPenalty, true);

        WrathIPCWrapper.SetAutoRotationConfigState(
            id,
            AutoRotationConfigOption.DPSRotationMode,
            manualTargeting ? DPSRotationMode.Manual : DPSRotationMode.Nearest);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.DPSAoETargets, 3);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.IgnoreRangeInBoss, true);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.FATEPriority, true);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.QuestPriority, false);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.OnlyAttackInCombat, false);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.DPSAlwaysHardTarget, true);

        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.HealerRotationMode, HealerRotationMode.Lowest_Current);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.SingleTargetHPP, 70);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.SingleTargetRegenHPP, 60);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.SingleTargetExcogHPP, 50);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.AoETargetHPP, 80);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.AutoRez, true);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.AutoRezOutOfParty, true);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.AutoRezDPSJobs, true);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.AutoRezDPSJobsHealersOnly, true);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.AutoCleanse, true);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.ManageKardia, true);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.IncludeNPCs, false);
        WrathIPCWrapper.SetAutoRotationConfigState(id, AutoRotationConfigOption.HealerAlwaysHardTarget, false);

        farmingDefaultsApplied = true;
    }

    // Official WrathCombo.API does not expose phantom-job helpers until the next package.
    private void TryLockOccultOptimal(Guid leaseId, uint phantomJobId)
    {
        try
        {
            string? parent = occult.GetOccultParentComboName(phantomJobId);
            if (string.IsNullOrEmpty(parent))
            {
                return;
            }

            occult.SetComboState(leaseId, parent, comboState: true, autoState: true);

            List<string>? options = occult.GetOccultOptionNames(phantomJobId);
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

                occult.SetComboOptionState(leaseId, option, state: true);
            }
        }
        catch
        {
        }
    }
}
