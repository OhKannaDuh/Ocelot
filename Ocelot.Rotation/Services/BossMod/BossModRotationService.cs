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
    // - Leylines helpers for BLM
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
                "BossMod.Autorotation.MiscAI.StayCloseToTarget": [
                  { "Track": "range", "Option": "{{rangeOption}}" }
                ]
              }
            }
            """;
    }

    private const string SINGLE_TARGET_PRESET =
        "eyJOYW1lIjoiT2NlbG90IFNpbmdsZSBUYXJnZXQiLCJNb2R1bGVzIjp7IkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5CTE0iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5SRE0iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5QQ1QiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5TTU4iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5EUkciOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5NTksiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5OSU4iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5SUFIiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5TQU0iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5WUFIiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5BU1QiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5TQ0giOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5XSE0iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5TR0UiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5CUkQiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5ETkMiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5NQ0giOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5EUksiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5HTkIiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5QTEQiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5CTFUiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLlZleW5CUkQiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLlZleW5XQVIiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV19fQ==";

    private bool bocchiAiReady;

    private bool singleTargetReady;

    public void Load()
    {
        Refresh();
    }

    public void Unload()
    {
        DestroyAutoRotationPreset();
    }

    public void Refresh()
    {
        // BOCCHI AI is ephemeral — only created while Illegal Mode is on.
        if (!singleTargetReady && ipc.IsAvailable)
        {
            var data = Convert.FromBase64String(SINGLE_TARGET_PRESET);
            singleTargetReady = ipc.Create(Encoding.UTF8.GetString(data), overwrite: true);
        }
    }

    public void EnsureAutoRotationPreset()
    {
        bocchiAiReady = EnsureBocchiAiPreset(overwrite: true);
    }

    public void DestroyAutoRotationPreset()
    {
        if (!ipc.IsAvailable)
        {
            bocchiAiReady = false;
            return;
        }

        ipc.ClearActive();
        ipc.Delete(AiPresetName);
        bocchiAiReady = false;
    }

    public void EnableAutoRotation()
    {
        if (!ipc.IsAvailable)
        {
            return;
        }

        // Recreate so role-specific StayCloseToTarget matches the current job, then activate.
        bocchiAiReady = EnsureBocchiAiPreset(overwrite: true);
        if (!bocchiAiReady)
        {
            return;
        }

        ipc.SetActive(AiPresetName);
    }

    public void DisableAutoRotation()
    {
        if (!ipc.IsAvailable)
        {
            return;
        }

        // Travel/pause: deactivate only — preset stays until Illegal Mode tears down.
        ipc.ClearActive();
    }

    public void EnableSingleTarget()
    {
        if (!ipc.IsAvailable)
        {
            return;
        }

        ipc.Activate(SINGLE_TARGET_PRESET_NAME);
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

        string json = BuildBocchiAiPresetJson(player.IsMelee());
        return ipc.Create(json, overwrite: true) || ipc.Get(AiPresetName) != null;
    }
}
