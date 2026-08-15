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

    void Prepare(CombatRotationRecipe recipe);

    void Enable(CombatActivity activity);

    void Disable();

    void Tick(uint? contentJobId = null);

    void Teardown();
}
