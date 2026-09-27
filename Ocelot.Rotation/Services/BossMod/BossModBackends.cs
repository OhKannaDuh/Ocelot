namespace Ocelot.Rotation.Services.BossMod;

public sealed class BossModMiscAiBackend(BossModPresetEngine engine) : ICombatAiBackend
{
    public CombatAiKind Kind => CombatAiKind.MiscAi;

    public void EnsurePresets() => engine.EnsurePresets(BossModPresetKind.MiscAi);

    public void Enable(CombatActivity activity) => engine.Enable(activity);

    public void Disable() => engine.Disable();

    public void Refresh() => engine.Refresh();

    public void Teardown() => engine.Teardown();

    public bool TryEnsurePresets(out string? storedJson) => engine.TryEnsureMiscAiPresets(out storedJson);
}

public sealed class BossModFullArJobRotation(BossModPresetEngine engine, JobRotationBackendKind kind) : IJobRotationBackend
{
    public JobRotationBackendKind Kind => kind;

    public void Prepare(JobRotationSessionOptions options)
    {
        _ = options;
        engine.EnsurePresets(BossModPresetKind.FullAr);
    }

    public void Enable(CombatActivity activity) => engine.Enable(activity);

    public void Disable() => engine.Disable();

    public void Refresh() => engine.Refresh();

    public void Teardown() => engine.Teardown();
}
