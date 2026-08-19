namespace Ocelot.Rotation.Services;

public interface IJobRotationBackend
{
    JobRotationBackendKind Kind { get; }

    void Prepare(JobRotationSessionOptions options);

    void Enable(CombatActivity activity);

    void Disable();

    void Refresh();

    void Teardown();

    void SyncContentJob(uint? contentJobId)
    {
    }
}

public interface ICombatAiBackend
{
    CombatAiKind Kind { get; }

    void EnsurePresets();

    void Enable(CombatActivity activity);

    void Disable();

    void Refresh();

    void Teardown();
}

public interface ICombatRotationSession
{
    CombatRotationRecipe ActiveRecipe { get; }

    /// <summary>
    ///     Rebuild owned BossMod FATE/CE presets from stock JSON. When false, existing presets are
    ///     kept and only created if missing.
    /// </summary>
    bool OverwriteBossModPresets { get; set; }

    bool BossModPresetsAvailable { get; }

    bool TryForceRecreateBossModPresets(BossModPresetKind kind);

    BossModMovementSettings MovementSettings { get; set; }

    void Prepare(CombatRotationRecipe recipe);

    void Enable(CombatActivity activity);

    void Disable();

    void Tick(uint? contentJobId = null);

    void Teardown();
}
