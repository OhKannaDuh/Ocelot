using Ocelot.Ipc.BossMod;
using Ocelot.Lifecycle;
using Ocelot.Services.PlayerState;

namespace Ocelot.Rotation.Services.BossMod;

public enum BocchiAiActivity
{
    Fate,
    CriticalEncounter,
}

public class BossModRotationService(IBossModIpc ipc, IPlayer player) : IRotationService, IOnPreUpdate
{
    public const string FatePresetName = "BOCCHI AI FATE";

    public const string CePresetName = "BOCCHI AI CE";

    /// <summary>Pre-split single preset — deleted on ensure/teardown.</summary>
    private const string LegacyAiPresetName = "BOCCHI AI";

    private const string LegacySingleTargetPresetName = "Ocelot Single Target";

    private bool wantBocchiAi;

    private bool wantActive;

    private bool bocchiAiReady;

    private BocchiAiActivity activeActivity = BocchiAiActivity.Fate;

    private BocchiAiActivity? armedActivity;

    private bool? bakedAsMelee;

    private uint? bakedJobId;

    public void PreUpdate() => Refresh();

    public void Load() => Refresh();

    public void Unload()
    {
        // Illegal Mode owns the ephemeral presets — ignore DynamicRotation provider churn.
        if (wantBocchiAi)
        {
            return;
        }

        if (ipc.IsAvailable)
        {
            DeactivateAllBocchiPresets();
        }

        ClearBakeState();
    }

    public void Refresh()
    {
        if (!wantBocchiAi)
        {
            return;
        }

        if (!ipc.IsAvailable)
        {
            bocchiAiReady = false;
            return;
        }

        uint jobId = CurrentJobId();
        bool isMelee = player.IsMelee();
        bool missing = ipc.Get(FatePresetName) == null || ipc.Get(CePresetName) == null;
        bool jobChanged = bakedJobId is not null && bakedJobId.Value != jobId;
        bool roleChanged = bakedAsMelee is not null && bakedAsMelee.Value != isMelee;
        string? fateStored = ipc.Get(FatePresetName);
        bool staleRange = fateStored != null && isMelee != fateStored.Contains("OnHitbox", StringComparison.Ordinal);

        if (!bocchiAiReady || missing || jobChanged || roleChanged || staleRange)
        {
            bool wasActive = wantActive || IsAnyBocchiPresetActive();
            bocchiAiReady = RecreateBocchiAiPresets(isMelee, jobId);
            if (bocchiAiReady && wasActive)
            {
                wantActive = true;
                ActivateCurrent();
                armedActivity = activeActivity;
            }
        }
    }

    public void EnsureAutoRotationPreset()
    {
        wantBocchiAi = true;
        DeleteLegacyPresets();
        bocchiAiReady = RecreateBocchiAiPresets(player.IsMelee(), CurrentJobId());
    }

    public void DestroyAutoRotationPreset()
    {
        wantBocchiAi = false;
        wantActive = false;
        armedActivity = null;
        ClearBakeState();

        if (!ipc.IsAvailable)
        {
            return;
        }

        DeactivateAllBocchiPresets();
        DeletePresetIfPresent(FatePresetName);
        DeletePresetIfPresent(CePresetName);
        DeleteLegacyPresets();
    }

    public void EnableAutoRotation() => EnableForActivity(BocchiAiActivity.Fate);

    public void EnableForActivity(BocchiAiActivity activity)
    {
        wantBocchiAi = true;
        bool alreadyArmed = wantActive && armedActivity == activity && bocchiAiReady;
        wantActive = true;
        activeActivity = activity;
        Refresh();
        if (!bocchiAiReady || !ipc.IsAvailable || alreadyArmed)
        {
            return;
        }

        ActivateCurrent();
        armedActivity = activity;
    }

    public void DisableAutoRotation()
    {
        if (!wantActive)
        {
            return;
        }

        wantActive = false;
        armedActivity = null;
        if (ipc.IsAvailable)
        {
            DeactivateAllBocchiPresets();
        }
    }

    public void EnableSingleTarget()
    {
    }

    public void DisableSingleTarget()
    {
    }

    public bool TryEnsureBocchiAiPreset(out string? storedJson)
    {
        storedJson = null;
        wantBocchiAi = true;
        DeleteLegacyPresets();
        bocchiAiReady = RecreateBocchiAiPresets(player.IsMelee(), CurrentJobId());
        if (!bocchiAiReady)
        {
            return false;
        }

        storedJson =
            $"=== {FatePresetName} ===\n{ipc.Get(FatePresetName)}\n\n=== {CePresetName} ===\n{ipc.Get(CePresetName)}";
        return true;
    }

    public static string PresetNameFor(BocchiAiActivity activity) =>
        activity == BocchiAiActivity.CriticalEncounter ? CePresetName : FatePresetName;

    private uint CurrentJobId() => player.GetClassJob()?.RowId ?? 0;

    private void ClearBakeState()
    {
        bocchiAiReady = false;
        bakedAsMelee = null;
        bakedJobId = null;
    }

    private void DeleteLegacyPresets()
    {
        DeletePresetIfPresent(LegacyAiPresetName);
        DeletePresetIfPresent(LegacySingleTargetPresetName);
    }

    private void DeletePresetIfPresent(string name)
    {
        if (!ipc.IsAvailable || ipc.Get(name) == null)
        {
            return;
        }

        ipc.Deactivate(name);
        ipc.Delete(name);
    }

    private void DeactivateAllBocchiPresets()
    {
        ipc.Deactivate(FatePresetName);
        ipc.Deactivate(CePresetName);
        ipc.Deactivate(LegacyAiPresetName);
    }

    private bool IsAnyBocchiPresetActive()
    {
        string? active = ipc.GetActive();
        return string.Equals(active, FatePresetName, StringComparison.Ordinal)
               || string.Equals(active, CePresetName, StringComparison.Ordinal)
               || string.Equals(active, LegacyAiPresetName, StringComparison.Ordinal);
    }

    private void ActivateCurrent()
    {
        string wanted = PresetNameFor(activeActivity);
        string other = activeActivity == BocchiAiActivity.Fate ? CePresetName : FatePresetName;
        ipc.Deactivate(other);
        ipc.Deactivate(LegacyAiPresetName);
        if (ipc.Activate(wanted))
        {
            return;
        }

        // Activate/SetActive returned false — one more SetActive attempt for BMR/VBM drift (#182).
        ipc.SetActive(wanted);
    }

    private bool RecreateBocchiAiPresets(bool isMelee, uint jobId)
    {
        if (!ipc.IsAvailable)
        {
            return false;
        }

        DeleteLegacyPresets();
        bool fateOk = UpsertPreset(FatePresetName, BuildBocchiAiPresetJson(FatePresetName, isMelee, forFate: true));
        bool ceOk = UpsertPreset(CePresetName, BuildBocchiAiPresetJson(CePresetName, isMelee, forFate: false));
        bool ok = fateOk && ceOk;
        if (ok)
        {
            bakedAsMelee = isMelee;
            bakedJobId = jobId;
        }
        else
        {
            ClearBakeState();
        }

        return ok;
    }

    private bool UpsertPreset(string name, string json)
    {
        if (ipc.Get(name) != null)
        {
            ipc.Deactivate(name);
            ipc.Delete(name);
        }

        ipc.Create(json, overwrite: true);
        return ipc.Get(name) != null;
    }

    private static string BuildBocchiAiPresetJson(string name, bool isMelee, bool forFate)
    {
        string rangeOption = isMelee ? "OnHitbox" : "15";
        string fateOption = forFate ? "Enabled" : "Disabled";
        string everythingOption = forFate ? "Disabled" : "Enabled";

        return
            $$"""
            {
              "Name": "{{name}}",
              "Modules": {
                "BossMod.Autorotation.MiscAI.AutoTarget": [
                  { "Track": "General", "Option": "Aggressive" },
                  { "Track": "Retarget", "Option": "Always" },
                  { "Track": "Treasure", "Option": "Disabled" },
                  { "Track": "FATE", "Option": "{{fateOption}}" },
                  { "Track": "Everything", "Option": "{{everythingOption}}" }
                ],
                "BossMod.Autorotation.MiscAI.StayWithinLeylines": [
                  { "Track": "Use Between The Lines", "Option": "Yes" },
                  { "Track": "Use Retrace", "Option": "Yes" }
                ],
                "BossMod.Autorotation.MiscAI.GoToPositional": [],
                "BossMod.Autorotation.MiscAI.StayCloseToTarget": [
                  { "Track": "range", "Option": "{{rangeOption}}" }
                ],
                "BossMod.Autorotation.MiscAI.NormalMovement": [
                  { "Track": "Destination", "Option": "Pathfind" },
                  { "Track": "ForbiddenZoneCushion", "Option": "None" },
                  { "Track": "Range", "Option": "Any" },
                  { "Track": "DelayMovement", "Option": "None" },
                  { "Track": "Cast", "Option": "Leeway" }
                ]
              }
            }
            """;
    }
}
