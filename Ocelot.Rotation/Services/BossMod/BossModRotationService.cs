using Ocelot.Ipc.BossMod;
using Ocelot.Lifecycle;
using Ocelot.Services.PlayerState;

namespace Ocelot.Rotation.Services.BossMod;

public class BossModRotationService(IBossModIpc ipc, IPlayer player) : IRotationService, IOnPreUpdate
{
    public const string AiPresetName = "BOCCHI AI";

    private const string LegacySingleTargetPresetName = "Ocelot Single Target";

    private bool wantBocchiAi;

    private bool wantActive;

    private bool bocchiAiReady;

    private bool? bakedAsMelee;

    private uint? bakedJobId;

    public void PreUpdate() => Refresh();

    public void Load() => Refresh();

    public void Unload()
    {
        // Illegal Mode owns the ephemeral preset — ignore DynamicRotation provider churn.
        if (wantBocchiAi)
        {
            return;
        }

        if (ipc.IsAvailable)
        {
            ipc.Deactivate(AiPresetName);
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
        string? stored = ipc.Get(AiPresetName);
        bool missing = stored == null;
        bool jobChanged = bakedJobId is not null && bakedJobId.Value != jobId;
        bool roleChanged = bakedAsMelee is not null && bakedAsMelee.Value != isMelee;
        bool staleRange = stored != null && isMelee != stored.Contains("OnHitbox", StringComparison.Ordinal);

        if (!bocchiAiReady || missing || jobChanged || roleChanged || staleRange)
        {
            bool wasActive = wantActive
                || string.Equals(ipc.GetActive(), AiPresetName, StringComparison.Ordinal);
            bocchiAiReady = RecreateBocchiAiPreset(isMelee, jobId);
            if (bocchiAiReady && wasActive)
            {
                wantActive = true;
                ipc.Activate(AiPresetName);
            }
        }
    }

    public void EnsureAutoRotationPreset()
    {
        wantBocchiAi = true;
        DeleteLegacySingleTargetPreset();
        bocchiAiReady = RecreateBocchiAiPreset(player.IsMelee(), CurrentJobId());
    }

    public void DestroyAutoRotationPreset()
    {
        wantBocchiAi = false;
        wantActive = false;
        ClearBakeState();

        if (!ipc.IsAvailable)
        {
            return;
        }

        ipc.Deactivate(AiPresetName);
        ipc.Delete(AiPresetName);
        DeleteLegacySingleTargetPreset();
    }

    public void EnableAutoRotation()
    {
        wantBocchiAi = true;
        wantActive = true;
        Refresh();
        if (bocchiAiReady && ipc.IsAvailable)
        {
            ipc.Activate(AiPresetName);
        }
    }

    public void DisableAutoRotation()
    {
        wantActive = false;
        if (ipc.IsAvailable)
        {
            ipc.Deactivate(AiPresetName);
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
        bocchiAiReady = RecreateBocchiAiPreset(player.IsMelee(), CurrentJobId());
        if (!bocchiAiReady)
        {
            return false;
        }

        storedJson = ipc.Get(AiPresetName);
        return true;
    }

    private uint CurrentJobId() => player.GetClassJob()?.RowId ?? 0;

    private void ClearBakeState()
    {
        bocchiAiReady = false;
        bakedAsMelee = null;
        bakedJobId = null;
    }

    private void DeleteLegacySingleTargetPreset()
    {
        if (!ipc.IsAvailable || ipc.Get(LegacySingleTargetPresetName) == null)
        {
            return;
        }

        ipc.Deactivate(LegacySingleTargetPresetName);
        ipc.Delete(LegacySingleTargetPresetName);
    }

    private bool RecreateBocchiAiPreset(bool isMelee, uint jobId)
    {
        if (!ipc.IsAvailable)
        {
            return false;
        }

        if (ipc.Get(AiPresetName) != null)
        {
            ipc.Deactivate(AiPresetName);
            ipc.Delete(AiPresetName);
        }

        ipc.Create(BuildBocchiAiPresetJson(isMelee), overwrite: true);
        bool ok = ipc.Get(AiPresetName) != null;
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

    private static string BuildBocchiAiPresetJson(bool isMelee)
    {
        string rangeOption = isMelee ? "OnHitbox" : "15";

        return
            $$"""
            {
              "Name": "BOCCHI AI",
              "Modules": {
                "BossMod.Autorotation.MiscAI.AutoTarget": [
                  { "Track": "General", "Option": "Aggressive" },
                  { "Track": "Retarget", "Option": "NoTarget" },
                  { "Track": "Treasure", "Option": "Disabled" },
                  { "Track": "FATE", "Option": "Enabled" }
                ],
                "BossMod.Autorotation.MiscAI.StayWithinLeylines": [
                  { "Track": "Use Between The Lines", "Option": "Yes" },
                  { "Track": "Use Retrace", "Option": "Yes" }
                ],
                "BossMod.Autorotation.MiscAI.NormalMovement": [
                  { "Track": "Destination", "Option": "Pathfind" }
                ],
                "BossMod.Autorotation.MiscAI.GoToPositional": [],
                "BossMod.Autorotation.MiscAI.StayCloseToTarget": [
                  { "Track": "range", "Option": "{{rangeOption}}" }
                ]
              }
            }
            """;
    }
}
