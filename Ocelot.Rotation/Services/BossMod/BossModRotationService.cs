using System.Text;
using Ocelot.Ipc.BossMod;
using Ocelot.Services.PlayerState;

namespace Ocelot.Rotation.Services.BossMod;

public class BossModRotationService(IBossModIpc ipc, IPlayer player) : IRotationService
{
    private const string SINGLE_TARGET_PRESET_NAME = "Ocelot Single Target";

    public const string AiPresetName = "BOCCHI AI";

    // Ephemeral soft AI for Illegal Mode FATEs/CEs (created on mode start, deleted on stop):
    // - AutoTarget FATE mobs without Always-retarget yanking after FATE knockbacks
    // - Pathfind + StayCloseToTarget (hitbox edge for melee, 15y for ranged)
    // - GoToPositional for melee (works with rotation plugins / RSR desired positional)
    // - Leylines helpers for BLM
    private static string BuildBocchiAiPresetJson(bool isMelee, bool includeGoToPositional)
    {
        string rangeOption = isMelee ? "OnHitbox" : "15";
        string goToPositional = isMelee && includeGoToPositional
            ? """
                "BossMod.Autorotation.MiscAI.GoToPositional": [
                  { "Track": "Positional", "Option": "Any" }
                ],
                """
            : string.Empty;

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
                {{goToPositional}}
                "BossMod.Autorotation.MiscAI.StayCloseToTarget": [
                  { "Track": "range", "Option": "{{rangeOption}}" }
                ]
              }
            }
            """;
    }

    private const string SINGLE_TARGET_PRESET =
        "eyJOYW1lIjoiT2NlbG90IFNpbmdsZSBUYXJnZXQiLCJNb2R1bGVzIjp7IkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5CTE0iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5SRE0iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5QQ1QiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5TTU4iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5EUkciOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5NTksiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5OSU4iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5SUFIiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5TQU0iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5WUFIiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5BU1QiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5TQ0giOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5XSE0iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5TR0UiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5CUkQiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5ETkMiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5NQ0giOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5EUksiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5HTkIiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5QTEQiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5CTFUiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLlZleW5CUkQiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLlZleW5XQVIiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV19fQ==";

    /// <summary>Illegal Mode asked us to keep BOCCHI AI around (create/retry until teardown).</summary>
    private bool wantBocchiAi;

    private bool bocchiAiReady;

    private bool singleTargetReady;

    public void Load()
    {
        Refresh();
    }

    public void Unload()
    {
        // Illegal Mode owns create/delete via Ensure/Destroy. Provider flicker must not wipe the preset.
        if (ipc.IsAvailable)
        {
            ipc.Deactivate(AiPresetName);
        }

        bocchiAiReady = false;
        singleTargetReady = false;
    }

    public void Refresh()
    {
        if (!ipc.IsAvailable)
        {
            return;
        }

        // BMR IPC is often not ready on the first Illegal Mode prepare — keep retrying.
        // Do this before any other Presets.Create so we don't lose the race to unrelated presets.
        if (wantBocchiAi && (ipc.Get(AiPresetName) == null || !bocchiAiReady))
        {
            bocchiAiReady = EnsureBocchiAiPreset(overwrite: ipc.Get(AiPresetName) == null);
        }
    }

    public void EnsureAutoRotationPreset()
    {
        wantBocchiAi = true;
        bocchiAiReady = EnsureBocchiAiPreset(overwrite: true);
    }

    public void DestroyAutoRotationPreset()
    {
        wantBocchiAi = false;
        bocchiAiReady = false;

        if (!ipc.IsAvailable)
        {
            return;
        }

        // Only drop BOCCHI AI — leave the user's other active presets alone.
        ipc.Deactivate(AiPresetName);
        ipc.Delete(AiPresetName);
    }

    public void EnableAutoRotation()
    {
        if (!ipc.IsAvailable)
        {
            return;
        }

        wantBocchiAi = true;

        // Recreate so role-specific StayCloseToTarget matches the current job, then activate.
        // VBM: Activate stacks with other presets. BMR: SetActive replaces the single slot.
        bocchiAiReady = EnsureBocchiAiPreset(overwrite: true);
        if (!bocchiAiReady)
        {
            return;
        }

        ipc.Activate(AiPresetName);
    }

    public void DisableAutoRotation()
    {
        if (!ipc.IsAvailable)
        {
            return;
        }

        // Travel/pause: deactivate BOCCHI AI only — preset stays until Illegal Mode tears down.
        ipc.Deactivate(AiPresetName);
    }

    public void EnableSingleTarget()
    {
        if (!ipc.IsAvailable)
        {
            return;
        }

        // Lazy-create — do not inject this preset on every Refresh (confused BMR testers; unused by Illegal Mode).
        if (!singleTargetReady)
        {
            var data = Convert.FromBase64String(SINGLE_TARGET_PRESET);
            singleTargetReady = ipc.Create(Encoding.UTF8.GetString(data), overwrite: true);
        }

        if (singleTargetReady)
        {
            ipc.Activate(SINGLE_TARGET_PRESET_NAME);
        }
    }

    public void DisableSingleTarget()
    {
        if (!ipc.IsAvailable)
        {
            return;
        }

        ipc.Deactivate(SINGLE_TARGET_PRESET_NAME);
    }

    /// <summary>
    ///     Create/overwrite <see cref="AiPresetName"/> and return the preset JSON BossMod stored (for debug).
    /// </summary>
    public bool TryEnsureBocchiAiPreset(out string? storedJson)
    {
        storedJson = null;
        wantBocchiAi = true;
        bocchiAiReady = EnsureBocchiAiPreset(overwrite: true);
        if (!bocchiAiReady)
        {
            return false;
        }

        storedJson = ipc.Get(AiPresetName);
        return true;
    }

    private bool EnsureBocchiAiPreset(bool overwrite)
    {
        if (!ipc.IsAvailable)
        {
            return false;
        }

        if (!overwrite && ipc.Get(AiPresetName) != null)
        {
            return true;
        }

        bool isMelee = player.IsMelee();

        // Full preset first; if Create/Get fails (seen more on BMR), retry without GoToPositional.
        if (TryCreateAndVerify(BuildBocchiAiPresetJson(isMelee, includeGoToPositional: true)))
        {
            return true;
        }

        if (isMelee && TryCreateAndVerify(BuildBocchiAiPresetJson(isMelee, includeGoToPositional: false)))
        {
            return true;
        }

        return ipc.Get(AiPresetName) != null;
    }

    private bool TryCreateAndVerify(string json)
    {
        if (!ipc.Create(json, overwrite: true))
        {
            // Create can return false even when Modify applied — trust Get.
            return ipc.Get(AiPresetName) != null;
        }

        return ipc.Get(AiPresetName) != null;
    }
}
