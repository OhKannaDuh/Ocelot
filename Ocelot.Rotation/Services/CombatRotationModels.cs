namespace Ocelot.Rotation.Services;

public enum CombatActivity
{
    Fate,
    CriticalEncounter,
}

public enum JobRotationBackendKind
{
    None = 0,
    Wrath = 1,
    RotationSolverReborn = 2,
    BossMod = 3,
    BossModReborn = 4,
}

public enum CombatAiKind
{
    None = 0,
    MiscAi = 1,
}

public enum BossModPresetKind
{
    MiscAi,
    FullAr,
}

/// <summary>StayCloseToTarget / NormalMovement options applied to BOCCHI's BossMod presets.</summary>
public readonly record struct BossModMovementSettings(
    string RangeOption,
    string ForbiddenZoneCushion,
    string DelayMovement)
{
    public static BossModMovementSettings Default { get; } = new("OnHitbox", "None", "None");
}

public static class JobRotationBackendKeys
{
    public const string Wrath = "WrathCombo";

    public const string RotationSolverReborn = "RotationSolver";

    public const string BossMod = "BossMod";

    public const string BossModReborn = "BossModReborn";
}

public readonly record struct JobRotationSessionOptions(bool ManualTargeting = true);

public readonly record struct CombatRotationRecipe(
    JobRotationBackendKind Job,
    CombatAiKind CombatAi,
    bool ManualTargeting = true
)
{
    public static CombatRotationRecipe None { get; } = new(JobRotationBackendKind.None, CombatAiKind.None);

    public bool IsActive => Job != JobRotationBackendKind.None || CombatAi != CombatAiKind.None;
}

/// <summary>BossMod preset display names. Created if missing, then left in the user's preset list.</summary>
public sealed class CombatAiPresetNaming
{
    public string FateMiscAi { get; init; } = "BOCCHI AI FATE";

    public string CeMiscAi { get; init; } = "BOCCHI AI CE";

    public string FateFullAr { get; init; } = "BOCCHI AR FATE";

    public string CeFullAr { get; init; } = "BOCCHI AR CE";

    public string PresetNameFor(CombatActivity activity, BossModPresetKind kind)
    {
        bool fate = activity == CombatActivity.Fate;
        return kind switch
        {
            BossModPresetKind.FullAr => fate ? FateFullAr : CeFullAr,
            _ => fate ? FateMiscAi : CeMiscAi,
        };
    }
}
