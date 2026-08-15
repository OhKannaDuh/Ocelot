using System.Text;
using Ocelot.Ipc.BossMod;
using Ocelot.Services.PlayerState;

namespace Ocelot.Rotation.Services.BossMod;

public sealed class BossModPresetEngine(IBossModIpc ipc, IPlayer player, CombatAiPresetNaming naming)
{
    private const string LegacyAiPresetName = "BOCCHI AI";

    private const string LegacySingleTargetPresetName = "Ocelot Single Target";

    private static readonly string[] XanRoleAiModules =
    [
        "BossMod.Autorotation.xan.MeleeAI",
        "BossMod.Autorotation.xan.RangedAI",
        "BossMod.Autorotation.xan.TankAI",
        "BossMod.Autorotation.xan.HealerAI",
        "BossMod.Autorotation.xan.Caster",
        "BossMod.Autorotation.xan.PhantomAI",
    ];

    private static readonly string[] XanStandardJobModules =
    [
        "BossMod.Autorotation.xan.DRG",
        "BossMod.Autorotation.xan.MNK",
        "BossMod.Autorotation.xan.NIN",
        "BossMod.Autorotation.xan.RPR",
        "BossMod.Autorotation.xan.SAM",
        "BossMod.Autorotation.xan.VPR",
        "BossMod.Autorotation.xan.DRK",
        "BossMod.Autorotation.xan.GNB",
        "BossMod.Autorotation.xan.PLD",
        "BossMod.Autorotation.xan.BRD",
        "BossMod.Autorotation.xan.DNC",
        "BossMod.Autorotation.xan.MCH",
        "BossMod.Autorotation.xan.AST",
        "BossMod.Autorotation.xan.SCH",
        "BossMod.Autorotation.xan.SGE",
        "BossMod.Autorotation.xan.WHM",
        "BossMod.Autorotation.xan.BLM",
        "BossMod.Autorotation.xan.PCT",
        "BossMod.Autorotation.xan.RDM",
        "BossMod.Autorotation.xan.SMN",
        "BossMod.Autorotation.VeynWAR",
    ];

    private bool wantOwned;

    private bool wantActive;

    private bool presetsReady;

    private CombatActivity activeActivity = CombatActivity.Fate;

    private BossModPresetKind presetKind = BossModPresetKind.MiscAi;

    private bool? bakedAsMelee;

    private uint? bakedJobId;

    public bool IsAvailable => ipc.IsAvailable;

    public void EnsurePresets(BossModPresetKind kind)
    {
        wantOwned = true;
        if (presetKind != kind)
        {
            DeleteOwnedPresets();
            ClearBakeState();
        }

        presetKind = kind;
        DeleteLegacyPresets();
        presetsReady = RecreatePresets(player.IsMelee(), CurrentJobId());
    }

    public void Enable(CombatActivity activity)
    {
        wantOwned = true;
        wantActive = true;
        activeActivity = activity;
        Refresh();
        if (presetsReady && ipc.IsAvailable)
        {
            ActivateCurrent();
        }
    }

    public void Disable()
    {
        wantActive = false;
        if (ipc.IsAvailable)
        {
            DeactivateAllOwnedPresets();
        }
    }

    public void Refresh()
    {
        if (!wantOwned)
        {
            return;
        }

        if (!ipc.IsAvailable)
        {
            presetsReady = false;
            return;
        }

        uint jobId = CurrentJobId();
        bool isMelee = player.IsMelee();
        string fate = FateName();
        string ce = CeName();
        bool missing = ipc.Get(fate) == null || ipc.Get(ce) == null;
        bool jobChanged = bakedJobId is not null && bakedJobId.Value != jobId;
        bool roleChanged = bakedAsMelee is not null && bakedAsMelee.Value != isMelee;
        string? fateStored = ipc.Get(fate);
        bool staleRange = fateStored != null && isMelee != fateStored.Contains("OnHitbox", StringComparison.Ordinal);

        if (!presetsReady || missing || jobChanged || roleChanged || staleRange)
        {
            bool wasActive = wantActive || IsAnyOwnedPresetActive();
            presetsReady = RecreatePresets(isMelee, jobId);
            if (presetsReady && wasActive)
            {
                wantActive = true;
                ActivateCurrent();
            }
        }
    }

    public void Teardown()
    {
        wantOwned = false;
        wantActive = false;
        ClearBakeState();

        if (!ipc.IsAvailable)
        {
            return;
        }

        DeactivateAllOwnedPresets();
        DeleteOwnedPresets();
        DeleteLegacyPresets();
        presetKind = BossModPresetKind.MiscAi;
    }

    public bool TryEnsureMiscAiPresets(out string? storedJson)
    {
        EnsurePresets(BossModPresetKind.MiscAi);
        storedJson = null;
        if (!presetsReady)
        {
            return false;
        }

        storedJson =
            $"=== {naming.FateMiscAi} ===\n{ipc.Get(naming.FateMiscAi)}\n\n=== {naming.CeMiscAi} ===\n{ipc.Get(naming.CeMiscAi)}";
        return true;
    }

    private string FateName() => naming.PresetNameFor(CombatActivity.Fate, presetKind);

    private string CeName() => naming.PresetNameFor(CombatActivity.CriticalEncounter, presetKind);

    private uint CurrentJobId() => player.GetClassJob()?.RowId ?? 0;

    private void ClearBakeState()
    {
        presetsReady = false;
        bakedAsMelee = null;
        bakedJobId = null;
    }

    private void DeleteLegacyPresets()
    {
        DeletePresetIfPresent(LegacyAiPresetName);
        DeletePresetIfPresent(LegacySingleTargetPresetName);
    }

    private void DeleteOwnedPresets()
    {
        DeletePresetIfPresent(naming.FateMiscAi);
        DeletePresetIfPresent(naming.CeMiscAi);
        DeletePresetIfPresent(naming.FateFullAr);
        DeletePresetIfPresent(naming.CeFullAr);
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

    private void DeactivateAllOwnedPresets()
    {
        ipc.Deactivate(naming.FateMiscAi);
        ipc.Deactivate(naming.CeMiscAi);
        ipc.Deactivate(naming.FateFullAr);
        ipc.Deactivate(naming.CeFullAr);
        ipc.Deactivate(LegacyAiPresetName);
    }

    private bool IsAnyOwnedPresetActive()
    {
        string? active = ipc.GetActive();
        return string.Equals(active, naming.FateMiscAi, StringComparison.Ordinal)
               || string.Equals(active, naming.CeMiscAi, StringComparison.Ordinal)
               || string.Equals(active, naming.FateFullAr, StringComparison.Ordinal)
               || string.Equals(active, naming.CeFullAr, StringComparison.Ordinal)
               || string.Equals(active, LegacyAiPresetName, StringComparison.Ordinal);
    }

    private void ActivateCurrent()
    {
        string wanted = naming.PresetNameFor(activeActivity, presetKind);
        string other = naming.PresetNameFor(
            activeActivity == CombatActivity.Fate
                ? CombatActivity.CriticalEncounter
                : CombatActivity.Fate,
            presetKind);
        ipc.Deactivate(other);
        ipc.Deactivate(LegacyAiPresetName);
        if (presetKind == BossModPresetKind.FullAr)
        {
            ipc.Deactivate(naming.FateMiscAi);
            ipc.Deactivate(naming.CeMiscAi);
        }
        else
        {
            ipc.Deactivate(naming.FateFullAr);
            ipc.Deactivate(naming.CeFullAr);
        }

        if (ipc.Activate(wanted))
        {
            return;
        }

        // Activate/SetActive returned false — one more SetActive attempt for BMR/VBM drift (#182).
        ipc.SetActive(wanted);
    }

    private bool RecreatePresets(bool isMelee, uint jobId)
    {
        if (!ipc.IsAvailable)
        {
            return false;
        }

        DeleteLegacyPresets();
        bool fateOk = UpsertPreset(FateName(), BuildPresetJson(FateName(), isMelee, forFate: true));
        bool ceOk = UpsertPreset(CeName(), BuildPresetJson(CeName(), isMelee, forFate: false));
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

    private string BuildPresetJson(string name, bool isMelee, bool forFate)
    {
        return presetKind == BossModPresetKind.FullAr
            ? BuildFullArPresetJson(name, isMelee, forFate)
            : BuildMiscAiPresetJson(name, isMelee, forFate);
    }

    private static string BuildMiscAiPresetJson(string name, bool isMelee, bool forFate)
    {
        string rangeOption = isMelee ? "OnHitbox" : "15";
        string fateOption = forFate ? "Enabled" : "Disabled";
        string everythingOption = forFate ? "Disabled" : "Enabled";

        return
            $$"""
            {
              "Name": "{{name}}",
              "Modules": {
                {{BuildMiscAiModulesJson(rangeOption, fateOption, everythingOption)}}
              }
            }
            """;
    }

    private static string BuildFullArPresetJson(string name, bool isMelee, bool forFate)
    {
        string rangeOption = isMelee ? "OnHitbox" : "15";
        string fateOption = forFate ? "Enabled" : "Disabled";
        string everythingOption = forFate ? "Disabled" : "Enabled";

        var sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine($"  \"Name\": \"{name}\",");
        sb.AppendLine("  \"Modules\": {");
        sb.Append(BuildMiscAiModulesJson(rangeOption, fateOption, everythingOption));
        sb.AppendLine(",");
        for (int i = 0; i < XanRoleAiModules.Length; i++)
        {
            sb.Append($"    \"{XanRoleAiModules[i]}\": []");
            sb.AppendLine(",");
        }

        for (int i = 0; i < XanStandardJobModules.Length; i++)
        {
            sb.Append($"    \"{XanStandardJobModules[i]}\": []");
            if (i < XanStandardJobModules.Length - 1)
            {
                sb.Append(',');
            }

            sb.AppendLine();
        }

        sb.AppendLine("  }");
        sb.Append('}');
        return sb.ToString();
    }

    private static string BuildMiscAiModulesJson(string rangeOption, string fateOption, string everythingOption) =>
        $$"""
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
        """;
}
