using Ocelot.Rotation.Services.BossMod;

namespace Ocelot.Rotation.Services;

public sealed class CombatRotationSession : ICombatRotationSession
{
    private readonly Dictionary<JobRotationBackendKind, IJobRotationBackend> jobs;

    private readonly Dictionary<CombatAiKind, ICombatAiBackend> ais;

    private readonly BossModPresetEngine bossModPresets;

    private readonly NullJobRotation nullJob = new();

    private readonly NullCombatAi nullAi = new();

    private IJobRotationBackend job;

    private ICombatAiBackend ai;

    public CombatRotationSession(
        IEnumerable<IJobRotationBackend> jobBackends,
        IEnumerable<ICombatAiBackend> combatAiBackends,
        BossModPresetEngine bossModPresets
    )
    {
        jobs = jobBackends.ToDictionary(b => b.Kind);
        ais = combatAiBackends.ToDictionary(b => b.Kind);
        this.bossModPresets = bossModPresets;
        job = nullJob;
        ai = nullAi;
    }

    public CombatRotationRecipe ActiveRecipe { get; private set; } = CombatRotationRecipe.None;

    public bool OverwriteBossModPresets
    {
        get => bossModPresets.OverwriteExisting;
        set => bossModPresets.OverwriteExisting = value;
    }

    public bool BossModPresetsAvailable => bossModPresets.IsAvailable;

    public bool TryForceRecreateBossModPresets(BossModPresetKind kind) =>
        bossModPresets.TryForceRecreate(kind);

    public BossModMovementSettings MovementSettings
    {
        get => bossModPresets.Movement;
        set => bossModPresets.Movement = value;
    }

    public void Prepare(CombatRotationRecipe recipe)
    {
        if (ActiveRecipe.IsActive)
        {
            Teardown();
        }

        ActiveRecipe = recipe;
        job = ResolveJob(recipe.Job);
        ai = ResolveAi(recipe.CombatAi);

        job.Prepare(new JobRotationSessionOptions(recipe.ManualTargeting));
        ai.EnsurePresets();
    }

    public void Enable(CombatActivity activity)
    {
        if (!ActiveRecipe.IsActive)
        {
            return;
        }

        ai.Enable(activity);
        job.Enable(activity);
    }

    public void Disable()
    {
        if (!ActiveRecipe.IsActive)
        {
            return;
        }

        job.Disable();
        ai.Disable();
    }

    public void Tick(uint? contentJobId = null)
    {
        if (!ActiveRecipe.IsActive)
        {
            return;
        }

        ai.Refresh();
        job.Refresh();
        job.SyncContentJob(contentJobId);
    }

    public void Teardown()
    {
        try
        {
            job.Teardown();
        }
        catch
        {
        }

        try
        {
            ai.Teardown();
        }
        catch
        {
        }

        job = nullJob;
        ai = nullAi;
        ActiveRecipe = CombatRotationRecipe.None;
    }

    private IJobRotationBackend ResolveJob(JobRotationBackendKind kind) =>
        kind != JobRotationBackendKind.None && jobs.TryGetValue(kind, out IJobRotationBackend? backend)
            ? backend
            : nullJob;

    private ICombatAiBackend ResolveAi(CombatAiKind kind) =>
        kind != CombatAiKind.None && ais.TryGetValue(kind, out ICombatAiBackend? backend)
            ? backend
            : nullAi;

    private sealed class NullJobRotation : IJobRotationBackend
    {
        public JobRotationBackendKind Kind => JobRotationBackendKind.None;

        public void Prepare(JobRotationSessionOptions options)
        {
        }

        public void Enable(CombatActivity activity)
        {
        }

        public void Disable()
        {
        }

        public void Refresh()
        {
        }

        public void Teardown()
        {
        }
    }

    private sealed class NullCombatAi : ICombatAiBackend
    {
        public CombatAiKind Kind => CombatAiKind.None;

        public void EnsurePresets()
        {
        }

        public void Enable(CombatActivity activity)
        {
        }

        public void Disable()
        {
        }

        public void Refresh()
        {
        }

        public void Teardown()
        {
        }
    }
}
